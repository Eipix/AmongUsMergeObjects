using UnityEngine;

public class AutoAspectWidthFitter : MonoBehaviour
{
    public float AspectRatio => (float)Screen.width / (float)Screen.height;

    private Vector2 _sourceScale;

    private void Awake() => _sourceScale = transform.localScale;

    private void Update() => transform.localScale = new Vector3((_sourceScale * AspectRatio).x, transform.localScale.y);
}
