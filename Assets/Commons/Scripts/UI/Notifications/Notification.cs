using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Notification : Singleton<Notification>
{
    [SerializeField] private Image _notify;

    private TMP_Text _text;
    private Sequence _sequence;

    public override void OnInit()
    {
        _text = _notify.GetComponentInChildren<TMP_Text>();
        _notify.gameObject.SetActive(false);
    }

    public void Notify(string text)
    {
        _sequence.CompleteIfActive();
        _text.text = text;
        _notify.gameObject.SetActive(true);
        (_notify.color, _text.color) = (_notify.color.Fade(), _notify.color.Fade());

        _sequence = DOTween.Sequence().SetUpdate(true)
                    .Append(_notify.DOFade(1f, 0.5f))
                    .Join(_text.DOFade(1f, 0.5f))
                    .AppendInterval(0.5f)
                    .Append(_notify.DOFade(0f, 1f))
                    .Join(_text.DOFade(0f, 1f))
                    .AppendCallback(() => _notify.gameObject.SetActive(false));
    }
}
