using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Code.Gameplay.Input.Behaviours
{
    public class SlideDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float swipeThreshold = 100f; // Минимальная длина свайпа
        [SerializeField] private float maxSwipeTime = 1f; // Максимальное время для свайпа

        private Vector2 _startPosition;
        private float _startTime;

        private InputContext _inputContext;

        [Inject]
        private void Construct
        (
            InputContext inputContext
        )
        {
            _inputContext = inputContext;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _startPosition = eventData.position;
            _startTime = Time.time;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            float swipeDuration = Time.time - _startTime;
            Vector2 swipeVector = eventData.position - _startPosition;

            if (swipeDuration <= maxSwipeTime && swipeVector.y < -swipeThreshold)
            {
                _inputContext.inputEntity.isJump = true;
                _inputContext.inputEntity.isJump = false;
            }
        }
    }
}