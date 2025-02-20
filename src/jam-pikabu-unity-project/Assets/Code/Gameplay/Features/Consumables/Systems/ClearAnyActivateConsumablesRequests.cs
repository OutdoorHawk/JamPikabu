using Entitas;

namespace Code.Gameplay.Features.Consumables.Systems
{
    public class ClearAnyActivateConsumablesRequests : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _requests;

        public ClearAnyActivateConsumablesRequests(GameContext context)
        {
            _requests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.ActivateConsumableRequest
                ));
        }

        public void Cleanup()
        {
            foreach (var entity in _requests)
            {
                entity.isDestructed = true;
            }
        }
    }
}