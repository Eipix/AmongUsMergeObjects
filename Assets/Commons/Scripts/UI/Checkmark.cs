using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Checkmark : MonoBehaviour
{
    [SerializeField] private AudioClip _onClickClip;
    [SerializeField] private Image _light;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _on;
    [SerializeField] private Sprite _off;

    public UnityEvent<bool> OnChecked;

    public bool State { get; set; }

    private void Start() => ChangeState();

    public void OnToggleClick()
    {
        if (_onClickClip != null)
            SoundsPlayer.Instance.PlayOneShotSound(_onClickClip);

        State = !State;
        ChangeState();
        OnChecked?.Invoke(State);
    }

    private void ChangeState()
    {
        if (State)
        {
            _image.sprite = _on;
            _light.gameObject.SetActive(false);
        }
        else
        {
            _image.sprite = _off;
            _light.gameObject.SetActive(true);
        }
    }
}
