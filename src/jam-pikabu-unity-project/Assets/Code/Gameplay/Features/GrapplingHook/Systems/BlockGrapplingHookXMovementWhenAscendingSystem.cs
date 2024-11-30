using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class BlockGrapplingHookXMovementWhenAscendingSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;

        public BlockGrapplingHookXMovementWhenAscendingSystem(GameContext gameContext)
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
            }
        }
    }
}