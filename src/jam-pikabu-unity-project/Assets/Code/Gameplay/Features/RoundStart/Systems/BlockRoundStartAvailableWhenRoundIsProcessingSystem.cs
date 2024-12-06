using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class BlockRoundStartAvailableWhenRoundIsProcessingSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;

        public BlockRoundStartAvailableWhenRoundIsProcessingSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController
                ));
        }

        public void Execute()
        {
            foreach (var entity in _roundController)
            {
                if (entity.isRoundInProcess)
                {
                    entity.isRoundStartAvailable = false;
                    continue;
                }
            }
        }
    }
}