using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Windows.Factory;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot
{
    public class GoldLootPickupSystem : IExecuteSystem
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IUIFactory _uiFactory;
        private readonly IGroup<GameEntity> _loot;
        private readonly Camera _camera;

        public GoldLootPickupSystem(GameContext context, ICurrencyFactory currencyFactory,
            IGameplayCurrencyService gameplayCurrencyService, IUIFactory uiFactory)
        {
            _currencyFactory = currencyFactory;
            _gameplayCurrencyService = gameplayCurrencyService;
            _uiFactory = uiFactory;

            _camera = Camera.main;

            _loot = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Loot,
                    GameMatcher.LootTypeId,
                    GameMatcher.CollectLootRequest,
                    GameMatcher.Gold
                ));
        }

        public void Execute()
        {
            foreach (var entity in _loot)
            {
                int goldAmount = entity.Gold;
                _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Gold, goldAmount, goldAmount);

                Vector3 startPosition = GetWorldPositionForUI(entity.Transform.position);

                var parameters = new CurrencyAnimationParameters
                {
                    Type = CurrencyTypeId.Gold,
                    Count = goldAmount,
                    StartPosition = startPosition,
                    EndPosition = _gameplayCurrencyService.Holder.PlayerCurrentGold.CurrencyIcon.transform.position,
                    StartReplenishCallback = () => _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Gold, 0, -goldAmount)
                };

                _currencyFactory.PlayCurrencyAnimation(parameters);
                entity.isDestructed = true;
            }
        }

        private Vector3 GetWorldPositionForUI(Vector3 worldPos)
        {
            // Преобразуем мировые координаты в экранные
            Vector3 screenPosition = _camera.WorldToScreenPoint(worldPos);

            // Преобразуем экранные координаты обратно в мировые координаты с учетом Canvas
            Canvas canvas = _uiFactory.UIRoot.GetComponent<Canvas>();
            RectTransform canvasRect = canvas.transform as RectTransform;

            // Учитываем положение и размер Canvas
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPosition, _camera, out Vector3 uiWorldPosition))
            {
                return uiWorldPosition;
            }

            Debug.LogWarning("Failed to convert world position to UI world position.");
            return Vector3.zero;
        }
    }
}