using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class BlockGrapplingHookMovementWhenCollectingLootSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;

        public BlockGrapplingHookMovementWhenCollectingLootSystem(GameContext gameContext)
        {
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.CollectingLoot
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