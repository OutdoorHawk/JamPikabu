using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class BlockRoundStartAvailableWhenRoundNotCompleteSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;

        public BlockRoundStartAvailableWhenRoundNotCompleteSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundOver
                ));
        }

        public void Execute()
        {
            foreach (var controller in _roundController)
            {
                if (controller.isRoundComplete == false)
                {
                    controller.isRoundStartAvailable = false;
                }
            }
        }
    }
}