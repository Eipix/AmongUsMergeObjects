using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Deadline : MonoBehaviour
{
    public event UnityAction Defeated;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.velocity.y < 0f)
            return;

        if (collision.TryGetComponent(out MergeObject mergeObject) && mergeObject.CanDefeat)
            Defeated?.Invoke();
    }

    [Button]
    private void InvokeDefeate() => Defeated?.Invoke();
}
