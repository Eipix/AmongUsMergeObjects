using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct Score
{
    [SerializeField] private TextMeshProUGUI _view;
    [SerializeField] private string _saveKey;

    private Tween _punch;
    private int _value;

    public TextMeshProUGUI View => _view;
    public string Key => _saveKey;
    public int Value 
    { 
        get => _value;
        set
        {
            if (value < 0)
                return;

            _value = value;
            _view.text = _value.ToString();
            _punch.CompleteIfActive();
            _punch = _view.rectTransform.DOPunchScale(Vector2.one * 0.5f, 0.3f);
        }
    }
}
