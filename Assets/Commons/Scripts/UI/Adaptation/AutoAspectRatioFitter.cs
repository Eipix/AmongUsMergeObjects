using UnityEngine;

public class AutoAspectRatioFitter : MonoBehaviour
{
    public float AspectRatio => (float)Screen.width / (float)Screen.height;

    private Vector2 _sourceScale;

    private void Awake() => _sourceScale = transform.localScale;

    private void Update() => transform.localScale = _sourceScale * AspectRatio;
}
