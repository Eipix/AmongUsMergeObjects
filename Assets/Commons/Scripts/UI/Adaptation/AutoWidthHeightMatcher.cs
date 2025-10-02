using UnityEngine;
using EmeraldPowder.CameraScaler;
using UnityEngine.Events;

[RequireComponent(typeof(CameraScaler))]
public class AutoWidthHeightMatcher : MonoBehaviour
{
    [Range(0.0f, 5.0f)]
    [SerializeField] private float _aspectToEnableAutoMatch;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _defaultMatch;
    [Range(-1.0f, 1.0f)]
    [SerializeField] private float _offset;

    public UnityEvent OnAlbumOrientation;
    public UnityEvent OnPortretOrientation;

    private CameraScaler _cameraScaler;

    public float AspectRatio => (float)Screen.width / (float)Screen.height;

    private void Awake() => _cameraScaler = GetComponent<CameraScaler>();

    private void Update()
    {
        if (AspectRatio <= _aspectToEnableAutoMatch)
            _cameraScaler.MatchWidthOrHeight = AspectRatio + _offset;
        else
            _cameraScaler.MatchWidthOrHeight = _defaultMatch;

        if (Screen.width > Screen.height)
        {
            OnAlbumOrientation?.Invoke();
        }
        else if (Screen.width < Screen.height)
        {;
            OnPortretOrientation?.Invoke();
        }
    }
}
