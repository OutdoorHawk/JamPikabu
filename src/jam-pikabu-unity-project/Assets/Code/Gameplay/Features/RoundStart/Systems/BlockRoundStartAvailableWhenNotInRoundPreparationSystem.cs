using Entitas;

namespace Code.Gameplay.Features.RoundStart.Systems
{
    public class BlockRoundStartAvailableWhenNotInRoundPreparationSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _gameState;

        public BlockRoundStartAvailableWhenNotInRoundPreparationSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController
                ));
            
            _gameState = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.GameState,
                    GameMatcher.RoundPreparation
                ));
        }

        public void Execute()
        {
            foreach (var entity in _roundController)
            {
                if (_gameState.count == 0)
                {
                    entity.isRoundStartAvailable = false;
                    continue;
                }
            }
        }
    }
}