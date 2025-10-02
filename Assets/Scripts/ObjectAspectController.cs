using UnityEngine;

public class ObjectAspectController : MonoBehaviour
{
    [SerializeField] private GameObject _controlled;
    [SerializeField] private float _maxAspectRatio;

    public float AspectRatio => (float)Screen.width / (float)Screen.height;

    private void Update() => _controlled.SetActive(AspectRatio >= _maxAspectRatio);
}
