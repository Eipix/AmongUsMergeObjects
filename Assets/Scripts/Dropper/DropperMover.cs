using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DropperMover : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    [SerializeField] private Dropper _dropper;
    [SerializeField] private Obstacle _leftWall;
    [SerializeField] private Obstacle _rightWall;
    [SerializeField] private Transform _platform;
    [SerializeField] private SpriteRenderer _spritePlatform;

    public event UnityAction OnPointerUped;
    public event UnityAction OnPointerDowned;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDowned?.Invoke();
        Move(eventData.pointerCurrentRaycast.worldPosition.x);
    }

    public void OnDrag(PointerEventData eventData) => Move(eventData.pointerCurrentRaycast.worldPosition.x);

    public void OnPointerUp(PointerEventData eventData) => OnPointerUped?.Invoke();

    public void OnPointerMove(PointerEventData eventData) => Move(eventData.pointerCurrentRaycast.worldPosition.x);

    public void FitInBorders()
    {
        AlignToTop();

        if (InMapBorders(_platform.position.x))
            return;

        if (CrossesLeftWall(_platform.position.x))
            SetPosition(Vector3.left, -_dropper.Current.WorldRadius, Mathf.Abs(_leftWall.RightSide));
        else if (CrossesRightWall(_platform.position.x))
            SetPosition(-Vector3.right, _dropper.Current.WorldRadius, - Mathf.Abs(_rightWall.LeftSide));
    }

    private void SetPosition(Vector3 direction, float offsetRadius, float obstacleOffset)
    {
        var delta = (_dropper.Current.transform.position.x + offsetRadius) + obstacleOffset;
        _platform.position += direction * delta;
    }

    private void Move(float targetPositionX)
    {
        var radius = _dropper.Current == null ? _spritePlatform.transform.localScale.x : _dropper.Current.WorldRadius;
        targetPositionX = Mathf.Clamp(targetPositionX, _leftWall.RightSide + radius, _rightWall.LeftSide - radius);
        _platform.position = new Vector2(targetPositionX, _platform.position.y);
    }

    private void AlignToTop()
    {
        var current = _dropper.Current;
        var distance = (_spritePlatform.transform.position.y - _spritePlatform.sprite.bounds.size.y / 2f) - current.WorldRadius;
        current.transform.position = new Vector3(current.transform.position.x, distance);
    }

    private bool InMapBorders(float posX)
    {
        if (CrossesLeftWall(posX) && posX >= _dropper.Current.transform.position.x)
            return false;

        if (CrossesRightWall(posX) && posX <= _dropper.Current.transform.position.x)
            return false;

        return true;
    }

    private bool CrossesLeftWall(float PosX) => PosX - _dropper.Current.WorldRadius <= _leftWall.RightSide;

    private bool CrossesRightWall(float PosX) => PosX + _dropper.Current.WorldRadius >= _rightWall.LeftSide;
}
