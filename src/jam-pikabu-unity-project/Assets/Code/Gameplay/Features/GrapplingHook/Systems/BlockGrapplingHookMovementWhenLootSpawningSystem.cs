using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class BlockGrapplingHookMovementWhenLootSpawningSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;
        private readonly IGroup<GameEntity> _spawners;

        public BlockGrapplingHookMovementWhenLootSpawningSystem(GameContext gameContext)
        {
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook
                ));
            
            _spawners = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootSpawner
                ));
        }

        public void Execute()
        {
            foreach (var _ in _spawners)
            foreach (var hook in _hooks)
            {
                hook.isXAxisMovementAvailable = false;
                hook.isYAxisMovementAvailable = false;
                hook.isDescentAvailable = false;
            }
        }
    }
}