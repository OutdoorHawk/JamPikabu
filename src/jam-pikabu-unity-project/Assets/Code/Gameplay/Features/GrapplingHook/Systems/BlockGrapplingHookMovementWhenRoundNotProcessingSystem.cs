using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class BlockGrapplingHookMovementWhenRoundNotProcessingSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;
        private readonly IGroup<GameEntity> _roundState;

        public BlockGrapplingHookMovementWhenRoundNotProcessingSystem(GameContext gameContext)
        {
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook
                ));
            
            _roundState = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks)
            {
                if (_roundState.GetEntities().Length != 0) 
                    continue;
                
                hook.isXAxisMovementAvailable = false;
                hook.isYAxisMovementAvailable = false;
                hook.isDescentAvailable = false;
            }
        }
    }
}