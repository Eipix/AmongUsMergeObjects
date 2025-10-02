using EmeraldPowder.CameraScaler;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasModeChanger : MonoBehaviour
{
    [Range(0.0f, 5.0f)]
    [SerializeField] private float _minAspectToChangeMode;

    private CanvasScaler _canvasScaler;

    public float AspectRatio => (float)Screen.width / (float)Screen.height;

    private void Awake() => _canvasScaler = GetComponent<CanvasScaler>();

    private void Update()
    {
        if (AspectRatio < _minAspectToChangeMode)
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        else
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    }
}
