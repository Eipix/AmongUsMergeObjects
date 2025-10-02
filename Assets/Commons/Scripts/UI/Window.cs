using UnityEngine;
using UnityEngine.UI;

public abstract class Window : MonoBehaviour
{
    [SerializeField] private Button _actionButton;

    private void OnEnable() => _actionButton.onClick.AddListener(OnButtonClick);

    private void OnDisable() => _actionButton.onClick.RemoveListener(OnButtonClick);

    public abstract void OnButtonClick();

    public abstract void Open();

    public abstract void Close();
}
