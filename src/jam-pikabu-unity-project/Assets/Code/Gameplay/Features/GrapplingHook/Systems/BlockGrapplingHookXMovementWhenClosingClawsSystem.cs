using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class BlockGrapplingHookXMovementWhenClosingClawsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;

        public BlockGrapplingHookXMovementWhenClosingClawsSystem(GameContext context)
        {
            _hooks = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.ClosingClaws
                ));
        }

        public void Execute()
        {
            foreach (var entity in _hooks)
            {
                entity.isXAxisMovementAvailable = false;
            }
        }
    }
}