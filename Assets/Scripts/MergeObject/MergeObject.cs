using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class MergeObject : MonoBehaviour
{
    [SerializeField, FoldoutGroup("References")]
    private Spawner _spawner;
    [SerializeField, FoldoutGroup("References")]
    private AudioClip _audioClip;

    [SerializeField, ChildGameObjectsOnly, FoldoutGroup("Component in childs")]
    private Rigidbody2D _rigidBody2D;
    [SerializeField, ChildGameObjectsOnly, FoldoutGroup("Component in childs")]
    private CircleCollider2D _collider2D;
    [SerializeField, ChildGameObjectsOnly, FoldoutGroup("Component in childs")]
    private SpriteRenderer _icon;
    [SerializeField, ChildGameObjectsOnly, FoldoutGroup("Component in childs")]
    private SpriteRenderer _bubbleRenderer;
    [SerializeField, ChildGameObjectsOnly, FoldoutGroup("Component in childs")]
    private ParticleSystem _spawnEffect;

    [SerializeField, FoldoutGroup("Config")]
    private int _score;
    [SerializeField, FoldoutGroup("Config")]
    private int _level;
    [SerializeField, FoldoutGroup("Config")]
    private float _jumpMagnitude;
    [SerializeField, FoldoutGroup("Config")]
    private float _defeatCooldownAfterJump;

    public event UnityAction<MergeObject, MergeObject, Vector2> OnNeighbourCollisition;

    private Timer _invincibleTimer;
    private Tween _showing;

    private Vector2 _defautScale;

    private int _defaultSortingOrderBubble;
    private int _defaultSortingOrderIcon;

    public Sprite Icon => _icon.sprite;

    public bool CanDefeat => _invincibleTimer == null;
    public int Score => _score;
    public int Level => _level;
    public float WorldRadius => _collider2D.radius * _defautScale.x;
    public string Data => $"{Level};{transform.position.x}:{transform.position.y}";

    public readonly float OffsetRadius = 0.1f;
    public readonly float ForceMagnitude = 3f;

    private void Awake()
    {
        _defaultSortingOrderBubble = _bubbleRenderer.sortingOrder;
        _defaultSortingOrderIcon = _icon.sortingOrder;
        _defautScale = transform.localScale;
        _spawnEffect.transform.localScale = Vector2.one * WorldRadius * 1.5f;
    }

    private void OnEnable() => OnNeighbourCollisition += _spawner.SpawnLevelUped;

    private void OnDisable() => OnNeighbourCollisition -= _spawner.SpawnLevelUped;

    public void Init(Spawner spawner) => _spawner = spawner;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out MergeObject neighbour) && neighbour.Level == Level)
        {
            Vector2 spawnPosition = (transform.position + neighbour.transform.position) / 2f;
            OnNeighbourCollisition?.Invoke(this, neighbour, spawnPosition);
        }
    }

    public void PlayShowing() => _showing = transform.DOScale(transform.localScale, 0.3f).From(Vector2.zero);

    public void OnLevelUp()
    {
        Activate();
        _spawnEffect.Play();
        PlayClip();
        ApplyRadialForce();
    }

    public void Disactivate()
    {
        _bubbleRenderer.sortingOrder = _defaultSortingOrderBubble + 2;
        _icon.sortingOrder = _defaultSortingOrderIcon + 2;
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _collider2D.enabled = false;
    }

    public void Activate()
    {
        _bubbleRenderer.sortingOrder = _defaultSortingOrderBubble;
        _icon.sortingOrder = _defaultSortingOrderIcon;
        _showing.CompleteIfActive();
        _rigidBody2D.constraints = RigidbodyConstraints2D.None;
        _collider2D.enabled = true;
    }

    private void PlayClip()
    {
        if (_audioClip == null)
            return;

        SoundsPlayer.Instance.StopAndPlayOneShotSound(_audioClip);
    }

    private void ApplyRadialForce()
    {
        Collider2D[] neighbours = Physics2D.OverlapCircleAll(transform.position, WorldRadius + OffsetRadius);

        foreach (var collider in neighbours)
        {
            Vector2 direction = (collider.transform.position - transform.position).normalized;
            collider.attachedRigidbody.AddForce(direction * ForceMagnitude, ForceMode2D.Impulse);
        }

        _rigidBody2D.AddForce(Vector2.up *  _jumpMagnitude, ForceMode2D.Impulse);
        _invincibleTimer = new Timer(_defeatCooldownAfterJump, () => _invincibleTimer = null);
        _invincibleTimer.Start();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, WorldRadius + OffsetRadius);
    //}
}
