using Entitas;

namespace Code.Gameplay.Features.Consumables.Systems
{
    public class ProcessConsumableInRoundLoopSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _roundLoop;

        public ProcessConsumableInRoundLoopSystem(GameContext context)
        {
            _roundLoop = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.GameState,
                    GameMatcher.RoundLoop
                ));

            _requests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.ActivateConsumableRequest
                ));
        }

        public void Execute()
        {
            foreach (var _ in _roundLoop)
            foreach (var entity in _requests)
            {
                entity.isProcessed = true;
            }
        }
    }
}