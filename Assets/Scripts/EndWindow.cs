using TMPro;
using UnityEngine;

public class EndWindow : PunchablePopup
{
    [SerializeField] private TextMeshProUGUI _score;

    private bool _isOpen;

    public void SetScore(string value) => _score.text = value;

    public override void Open()
    {
        if (_isOpen)
            return;

        _isOpen = true;
        base.Open();
    }

    public override void Close()
    {
        if(_isOpen == false)
            return;

        _isOpen = false;
        base.Close();
    }
}
