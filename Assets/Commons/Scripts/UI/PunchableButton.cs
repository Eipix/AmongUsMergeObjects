using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PunchableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private AudioClip _clip;
    [Range(0, 1)]
    [SerializeField] private float _punchStrength;
    [Range(0, 1)]
    [SerializeField] private float _time;

    private RectTransform _rectTransform;
    private Tween _tween;
    private Vector2 _defaultScale;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
        _defaultScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData) => ScaleDown();

    public void OnPointerUp(PointerEventData eventData) => ScaleUp();

    private void ScaleDown()
    {
        PlayClip();
        _tween.CompleteIfActive(true);
        _tween = _rectTransform.DOScale(_defaultScale * (1 - _punchStrength), _time).SetUpdate(true);
    }

    private void ScaleUp()
    {
        _tween.CompleteIfActive(true);
        _tween = _rectTransform.DOScale(_defaultScale, _time).SetUpdate(true);
    }

    private void PlayClip()
    {
        if(_clip != null)
            SoundsPlayer.Instance.PlayOneShotSound(_clip);
    }
}
