using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Animations;

public class DirectionAnimation : MonoBehaviour
{
    [SerializeField] private Axis _moveAxis;
    [SerializeField] private float _targetPosition;
    [SerializeField] private SpriteRenderer _dublicate;
    [SerializeField] private float _duration;

    private Sequence _sequence;

    private void Start()
    {
        _sequence = DOTween.Sequence();
        Tween tween = null;

        switch (_moveAxis)
        {
            case Axis.X:
                tween = _dublicate.transform.DOMoveX(_targetPosition, _duration);
                break;
            case Axis.Y:
                tween = _dublicate.transform.DOMoveY(_targetPosition, _duration);
                break;
            default:
                break;
        }
        
        if (tween == null)
            return;

        _sequence.Append(tween.SetEase(Ease.Linear));
        _sequence.SetLoops(-1, LoopType.Restart);
    }

    public void Hide() => _dublicate.gameObject.SetActive(false);
    public void Show() => _dublicate.gameObject.SetActive(true);
}
