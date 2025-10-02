using UnityEngine;
using System.Collections;

public class Dropper : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _platform;
    [SerializeField] private DirectionAnimation _animation;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private DropperMover _mover;
    [SerializeField] private Game _game;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private AudioClip _drop;
    [SerializeField] private float _offset;

    [Range(0f, 1f)]
    [SerializeField] private float _dropDelay = .5f;

    private MergeObject _current;
    private Coroutine _dropping;

    private bool _droppingActive => _dropping != null;

    public MergeObject Current => _current;

    public Transform SpawnPoint => _spawnPoint;

    private void OnEnable() => _mover.OnPointerUped += Drop;

    private void OnDisable() => _mover.OnPointerUped -= Drop;

    public void Next()
    {
        if (_droppingActive)
            return;

        ChangeCurrent();
    }

    private void ChangeCurrent()
    {
        _animation.Show();
        _current = _spawner.SpawnRandom();
        _current.Disactivate();
        _mover.FitInBorders();
    }

    private void Drop()
    {
        if (_dropping != null)
            return;

        _dropping = StartCoroutine(DropRoutine());
    }

    private IEnumerator DropRoutine()
    {
        SoundsPlayer.Instance.PlayOneShotSound(_drop);
        _animation.Hide();
        _current.Activate();
        _current.transform.SetParent(_spawner.transform);
        _current = null;

        yield return new WaitForSeconds(_dropDelay);
        ChangeCurrent();
        _dropping = null;
    }
}
