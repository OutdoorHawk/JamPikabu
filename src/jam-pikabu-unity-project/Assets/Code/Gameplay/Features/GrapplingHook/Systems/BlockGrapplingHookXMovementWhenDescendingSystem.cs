using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class BlockGrapplingHookXMovementWhenDescendingSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;

        public BlockGrapplingHookXMovementWhenDescendingSystem(GameContext gameContext)
        {
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.Descending
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks)
            {
                hook.isXAxisMovementAvailable = false;
                hook.isDescentAvailable = false;
            }
        }
    }
}