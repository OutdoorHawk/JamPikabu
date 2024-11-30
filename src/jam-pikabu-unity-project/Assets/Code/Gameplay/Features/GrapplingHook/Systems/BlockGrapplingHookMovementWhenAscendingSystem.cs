using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class BlockGrapplingHookMovementWhenAscendingSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;

        public BlockGrapplingHookMovementWhenAscendingSystem(GameContext gameContext)
        {
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.AscentRequested,
                    GameMatcher.AscentAvailable
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks)
            {
                hook.isXAxisMovementAvailable = false;
                
                hook.isDescentAvailable = false;
                hook.isDescentRequested = false;
            }
        }
    }
}