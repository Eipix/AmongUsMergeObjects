using UnityEngine;
using UnityEngine.EventSystems;

namespace Eipix.UI
{
    [RequireComponent(typeof(Swiper))]
    public class Swipes : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private SwipeMethod _swipeMethod;

        private Swiper _swiper;

        private Vector2 _direction;

        public enum SwipeMethod
        {
            Horizontal,
            Vertical
        }

        private void Awake() => _swiper = GetComponent<Swiper>();

        public void OnBeginDrag(PointerEventData eventData) => _direction = GetDirection(eventData.delta);
        
        public void OnDrag(PointerEventData eventData) { }

        public void OnEndDrag(PointerEventData eventData)
        {
            switch (_swipeMethod)
            {
                case SwipeMethod.Horizontal:
                    Move(_direction.x);
                    break;
                case SwipeMethod.Vertical:
                    Move(_direction.y);
                    break;
                default:
                    break;
            }
        }

        private Vector2 GetDirection(Vector2 vector)
        {
            if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
                return vector.x > 0 ? Vector2.right : Vector2.left;
            else
                return vector.y > 0 ? Vector2.up : Vector2.down;
        }

        private void Move(float axis)
        {
            if (axis > 0)
            {
                _swiper.MoveBack();
            }
            else if (axis != 0)
            {
                _swiper.MoveForward();
            }
        }
    }
}
