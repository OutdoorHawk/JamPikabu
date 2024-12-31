using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
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
        private readonly ISoundService _soundService;
        private readonly IGroup<GameEntity> _loot;

        public GoldLootPickupSystem(GameContext context, ICurrencyFactory currencyFactory,
            IGameplayCurrencyService gameplayCurrencyService, IUIFactory uiFactory, ISoundService soundService)
        {
            _currencyFactory = currencyFactory;
            _gameplayCurrencyService = gameplayCurrencyService;
            _uiFactory = uiFactory;
            _soundService = soundService;

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

                Vector3 startPosition = _uiFactory.GetWorldPositionForUI(entity.Transform.position);

                var parameters = new CurrencyAnimationParameters
                {
                    TextPrefix = "+",
                    Type = CurrencyTypeId.Gold,
                    Count = goldAmount,
                    StartPosition = startPosition,
                    EndPosition = _gameplayCurrencyService.Holder.PlayerCurrentGold.CurrencyIcon.transform.position,
                    StartReplenishCallback = () => _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Gold, 0, -goldAmount)
                };

                _currencyFactory.PlayCurrencyAnimation(parameters);
                entity.isDestructed = true;
                _soundService.PlaySound(SoundTypeId.Gold_Currency_Collect);
            }
        }
    }
}