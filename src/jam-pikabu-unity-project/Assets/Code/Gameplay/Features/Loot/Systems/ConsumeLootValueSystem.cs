using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ConsumeLootValueSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _readyLoot;
        private readonly IGroup<GameEntity> _lootApplier;
        private readonly List<GameEntity> _buffer = new(64);

        private readonly IOrdersService _ordersService;
        private readonly ICurrencyFactory _currencyFactory;

        public ConsumeLootValueSystem(GameContext context, IOrdersService ordersService, ICurrencyFactory currencyFactory)
        {
            _currencyFactory = currencyFactory;
            _ordersService = ordersService;

            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));

            _readyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.Rating
                ).NoneOf(
                    GameMatcher.Consumed));
        }

        public void Execute()
        {
            foreach (var _ in _lootApplier)
            {
                ApplyOrderRatingFactors();
                
                foreach (var loot in _readyLoot.GetEntities(_buffer))
                {
                    loot.isConsumed = true;

                    if (_ordersService.TryGetIngredientData(loot.LootTypeId, out IngredientData data) == false)
                        continue;

                    int ratingAmount = loot.Rating * data.RatingFactor;
                    _currencyFactory.CreateAddCurrencyRequest(data.RatingType, ratingAmount, ratingAmount);
                }
            }
        }

        private void ApplyOrderRatingFactors()
        {
            if (_readyLoot.count == 0)
                return;
            
            if (_ordersService.TryGetBonusRating(out int ratingAmount) == false)
                return;
            
            _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Plus, ratingAmount, ratingAmount);
        }
    }
}