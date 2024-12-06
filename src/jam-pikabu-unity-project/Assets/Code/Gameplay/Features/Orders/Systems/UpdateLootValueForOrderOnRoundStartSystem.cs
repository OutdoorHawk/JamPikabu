using System.Collections.Generic;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class UpdateLootValueForOrderOnRoundStartSystem : ReactiveSystem<GameEntity>
    {
        private readonly IOrdersService _ordersService;
        private readonly IGroup<GameEntity> _loot;

        public UpdateLootValueForOrderOnRoundStartSystem(GameContext context, IOrdersService ordersService) : base(context)
        {
            _ordersService = ordersService;

            _loot = context.GetGroup(GameMatcher.AllOf(
                GameMatcher.Loot));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.RoundStateController,
                GameMatcher.RoundInProcess).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isRoundStateController && entity.isRoundInProcess;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            GameEntity order = _ordersService.CreateOrder();
            OrderSetup orderSetup = order.OrderData.Setup;

            foreach (var loot in _loot)
            {
                if (loot.hasPlus)
                    loot.RemovePlus();
                
                if (loot.hasMinus)
                    loot.RemoveMinus();
            }

            foreach (GameEntity loot in _loot)
            foreach (var ingredientData in orderSetup.GoodIngredients)
            {
                if (ingredientData.TypeId == loot.LootTypeId)
                {
                    loot.ReplacePlus(ingredientData.Rating.Amount);
                }
            }

            foreach (GameEntity loot in _loot)
            foreach (var ingredientData in orderSetup.BadIngredients)
            {
                if (ingredientData.TypeId == loot.LootTypeId)
                    loot.ReplaceMinus(ingredientData.Rating.Amount);
            }
        }
    }
}