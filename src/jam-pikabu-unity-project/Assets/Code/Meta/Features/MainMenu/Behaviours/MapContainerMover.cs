using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class MapContainerMover : MonoBehaviour
    {
        public ScrollRect MainScroll;
        public Button LeftButton;
        public Button RightButton;
        public MapContainer MapContainer;
        public float MoveThreshold = 100;
        public float MoveDuration = 0.5f;

        private Tween _moveTween;
        private float _cellSizeX;
        private float _spacing;
        private float _currentMoveValue;

        private RectTransform[] _segments;

        private const int SEGMENTS_ON_SCREEN = 3;

        private int _currentSegment;
        private int _totalSegments;

        private void Awake()
        {
            LeftButton.onClick.AddListener(MoveLeft);
            RightButton.onClick.AddListener(MoveRight);
        }

        private void Start()
        {
            Initialize();
            UpdateButtonInteractivity();
        }

        private void Update()
        {
            UpdateMoveValue();
            TryResetToCurrentPosition();
            TryMoveByThreshold();
        }

        private void OnDestroy()
        {
            LeftButton.onClick.RemoveListener(MoveLeft);
            RightButton.onClick.RemoveListener(MoveRight);
        }

        private void Initialize()
        {
            HorizontalLayoutGroup layoutGroup = MapContainer.GetComponent<HorizontalLayoutGroup>();
            _spacing = layoutGroup.spacing;
            _cellSizeX = MapContainer.MapBlocks[0].GetComponent<RectTransform>().rect.width + _spacing;

            _totalSegments = MapContainer.MapBlocks.Count;
            _segments = new RectTransform[_totalSegments];

            for (int i = 0; i < _totalSegments; i++)
            {
                _segments[i] = MapContainer.MapBlocks[i].GetComponent<RectTransform>();
            }
        }

        private void UpdateMoveValue()
        {
            _currentMoveValue = MainScroll.velocity.x;
        }

        private void TryResetToCurrentPosition()
        {
            if (_moveTween != null || Mathf.Abs(_currentMoveValue) > 0)
                return;

            Vector2 targetPosition = CalculateTargetPosition(_currentSegment);

            _moveTween?.Kill();
            _moveTween = MainScroll.content
                .DOAnchorPos(targetPosition, MoveDuration)
                .SetLink(gameObject)
                .SetEase(Ease.OutQuad);
        }

        private void TryMoveByThreshold()
        {
            if (_moveTween != null || Mathf.Abs(_currentMoveValue) < MoveThreshold)
                return;

            if (_currentMoveValue > 0)
                MoveLeft();
            else
                MoveRight();
        }

        private void MoveLeft()
        {
            if (_currentSegment <= 0)
                return;

            _moveTween?.Kill();
            _currentSegment--;

            Vector2 targetPosition = CalculateTargetPosition(_currentSegment);
            _moveTween = MainScroll.content
                .DOAnchorPos(targetPosition, MoveDuration)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);

            UpdateButtonInteractivity();
        }

        private void MoveRight()
        {
            if (_currentSegment >= _totalSegments - SEGMENTS_ON_SCREEN)
                return;

            _moveTween?.Kill();
            _currentSegment++;

            Vector2 targetPosition = CalculateTargetPosition(_currentSegment);
            _moveTween = MainScroll.content
                .DOAnchorPos(targetPosition, MoveDuration)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);

            UpdateButtonInteractivity();
        }

        private Vector2 CalculateTargetPosition(int segmentIndex)
        {
            float targetX = -segmentIndex * _cellSizeX;
            return new Vector2(targetX, MainScroll.content.anchoredPosition.y);
        }

        private void UpdateButtonInteractivity()
        {
            LeftButton.interactable = _currentSegment > 0;
            RightButton.interactable = _currentSegment < _totalSegments - SEGMENTS_ON_SCREEN;
        }
    }
}