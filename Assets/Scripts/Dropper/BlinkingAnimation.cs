using DG.Tweening;
using UnityEngine;

public class BlinkingAnimation : MonoBehaviour
{
    [SerializeField] private DropperMover _mover;
    [SerializeField] private SpriteRenderer[] _animatedRenders;
    [SerializeField] private float _durationOfFade;

    [Range(0f, 1f)]
    [SerializeField] private float _endValue;

    private Sequence _sequence;

    private float[] _defaultFades;

    private void Awake()
    {
        _defaultFades = new float[_animatedRenders.Length];

        for (int i = 0; i < _defaultFades.Length; i++)
            _defaultFades[i] = _animatedRenders[i].color.a;
    }

    private void OnEnable()
    {
        _mover.OnPointerDowned += Play;
        _mover.OnPointerUped += Reset;
    }

    private void OnDisable()
    {
        _mover.OnPointerDowned -= Play;
        _mover.OnPointerUped -= Reset;
    }

    public void Play()
    {
        _sequence = DOTween.Sequence();
        for (int i = 0; i < _animatedRenders.Length; i++)
        {
            _sequence.Insert(0, _animatedRenders[i].DOFade(_endValue, _durationOfFade));
            _sequence.Insert(1, _animatedRenders[i].DOFade(1f, _durationOfFade));
        }
        _sequence.SetLoops(-1);
        _sequence.OnKill(SetDefaultState);
    }

    public void Reset() => _sequence.Kill();

    private void SetDefaultState()
    {
        for (int i = 0; i < _animatedRenders.Length; i++)
        {
            _animatedRenders[i].color = _animatedRenders[i].color.Fade(_defaultFades[i]);
        }
    }
}
