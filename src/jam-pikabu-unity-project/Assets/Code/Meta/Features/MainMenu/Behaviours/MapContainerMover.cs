using Code.Common.Extensions;
using Code.Gameplay.Features.Loot;
using Code.Meta.Features.LootCollection.Service;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class MapContainerMover : MonoBehaviour
    {
        public ScrollRect MainScroll;
        public Button LeftButton;
        public Button RightButton;
        public GameObject LeftPin;
        public GameObject RightPin;
        public float MoveThreshold = 100;
        public float MoveDuration = 0.5f;

        private ILootCollectionService _lootCollectionService;
        
        private Tween _moveTween;
        private float _cellSizeX;
        private float _spacing;
        private float _currentMoveValue;

        private MapBlock[] _segments;

        private const int SEGMENTS_ON_SCREEN = 3;

        private int _currentSegment;
        private int _totalSegments;

        [Inject]
        private void Construct(ILootCollectionService lootCollectionService)
        {
            _lootCollectionService = lootCollectionService;
        }

        private void Awake()
        {
            LeftButton.onClick.AddListener(MoveLeft);
            RightButton.onClick.AddListener(MoveRight);
            _lootCollectionService.OnFreeUpgradeTimeEnd += UpdatePin;
        }

        private void OnDestroy()
        {
            LeftButton.onClick.RemoveListener(MoveLeft);
            RightButton.onClick.RemoveListener(MoveRight);
            _lootCollectionService.OnFreeUpgradeTimeEnd -= UpdatePin;
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

        private void Initialize()
        {
            HorizontalLayoutGroup layoutGroup = MainScroll.content.GetComponent<HorizontalLayoutGroup>();
            MapBlock[] mapBlocks = MainScroll.content.GetComponentsInChildren<MapBlock>(true);
            _spacing = layoutGroup.spacing;
            _cellSizeX = mapBlocks[0].GetComponent<RectTransform>().rect.width + _spacing;

            _totalSegments = mapBlocks.Length;
            _segments = new MapBlock[_totalSegments];

            for (int i = 0; i < _totalSegments; i++) 
                _segments[i] = mapBlocks[i];
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
            UpdatePin();
        }
        
        private void UpdatePin()
        {
            LeftPin.DisableElement();
            RightPin.DisableElement();
            
            for (int i = 0; i < _segments.Length; i++)
            {
                MapBlock mapBlock = _segments[i];
                LootTypeId type = mapBlock.UnlockableIngredient.UnlocksIngredient;

                if (_lootCollectionService.CanUpgradeForFree(type) == false)
                    continue;

                if (_lootCollectionService.GetTimeLeftToFreeUpgrade(type) > 0)
                    continue;
                
                // Определяем позицию mapBlock относительно видимой области
                if (i < _currentSegment) // Левее видимой области
                {
                    LeftPin.EnableElement();
                    return;
                }

                if (i >= _currentSegment + SEGMENTS_ON_SCREEN) // Правее видимой области
                {
                    RightPin.EnableElement();
                    return;
                }
            }
        }
    }
}