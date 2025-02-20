using Code.Meta.Features.Consumables.Service;
using Entitas;

namespace Code.Gameplay.Features.Consumables.Systems
{
    public class ProcessConsumableSpendSystem : IExecuteSystem
    {
        private readonly IConsumablesUIService _consumablesUIService;
        private readonly IGroup<GameEntity> _processedConsumables;

        public ProcessConsumableSpendSystem(GameContext context, IConsumablesUIService consumablesUIService)
        {
            _consumablesUIService = consumablesUIService;
            
            _processedConsumables = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.ActivateConsumableRequest,
                    GameMatcher.ConsumableTypeId,
                    GameMatcher.Processed
                ));
        }

        public void Execute()
        {
            foreach (var entity in _processedConsumables)
            {
                _consumablesUIService.ConsumableSpend(entity.ConsumableTypeId);
            }
        }
    }
}