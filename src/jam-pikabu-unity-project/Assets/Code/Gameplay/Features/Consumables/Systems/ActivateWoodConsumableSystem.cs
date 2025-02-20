using Entitas;

namespace Code.Gameplay.Features.Consumables.Systems
{
    public class ActivateWoodConsumableSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _otherLoot;

        public ActivateWoodConsumableSystem(GameContext context)
        {
            _requests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.ActivateConsumableRequest,
                    GameMatcher.Wood,
                    GameMatcher.Processed
                ));
        }

        public void Execute()
        {
            foreach (var _ in _requests)
            {
                
            }
        }
    }
}