using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _collider2D;

    public BoxCollider2D BoxCollider2D => _collider2D;

    public Vector2 WorldPosition => (Vector2)transform.position + _collider2D.offset;

    public float LeftSide => WorldPosition.x - (_collider2D.bounds.size.x / 2f);

    public float RightSide => WorldPosition.x + (_collider2D.bounds.size.x / 2f);
}
