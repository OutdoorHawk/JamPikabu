using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Currency.Service;
using Entitas;

namespace Code.Gameplay.Features.Loot
{
    public class GoldLootPickupSystem : IExecuteSystem
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IGroup<GameEntity> _loot;

        public GoldLootPickupSystem(GameContext context, ICurrencyFactory currencyFactory, IGameplayCurrencyService gameplayCurrencyService)
        {
            _currencyFactory = currencyFactory;
            _gameplayCurrencyService = gameplayCurrencyService;
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

                var parameters = new CurrencyAnimationParameters
                {
                    Type = CurrencyTypeId.Gold,
                    Count = goldAmount,
                    StartPosition = entity.Transform.position,
                    EndPosition = _gameplayCurrencyService.Holder.PlayerCurrentGold.CurrencyIcon.transform.position,
                    StartReplenishCallback = () => _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Gold, 0, -goldAmount)
                };

                _currencyFactory.PlayCurrencyAnimation(parameters);
                entity.isDestructed = true;
            }
        }
    }
}