using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Loot.Service;
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

        private readonly ILootService _lootService;
        private readonly IOrdersService _ordersService;

        public ConsumeLootValueSystem(GameContext context, ILootService lootService, IOrdersService ordersService)
        {
            _lootService = lootService;
            _ordersService = ordersService;
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));

            _readyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.LootItemUI
                ).NoneOf(
                    GameMatcher.Consumed));
        }

        public void Execute()
        {
            foreach (var _ in _lootApplier)
            foreach (var loot in _readyLoot.GetEntities(_buffer))
            {
                loot.isConsumed = true;

                if (_ordersService.TryGetIngredientData(loot.LootTypeId, out IngredientData data))
                {
                    //TODO CALCULATE RATING WITH ORDER FACTOR
                    
                    /*CreateGameEntity.Empty()
                        .With(x => x.isAddCurrencyRequest = true)
                        .With(x => x.AddPlus(data.Rating.Amount), when: data.Rating.CurrencyType == CurrencyTypeId.Plus)
                        .With(x => x.AddMinus(data.Rating.Amount), when: data.Rating.CurrencyType == CurrencyTypeId.Minus)
                        .With(x => x.AddWithdraw(data.Rating.Amount), when: data.Rating.Amount > 0)
                        ;*/
                }
            }
        }
    }
}