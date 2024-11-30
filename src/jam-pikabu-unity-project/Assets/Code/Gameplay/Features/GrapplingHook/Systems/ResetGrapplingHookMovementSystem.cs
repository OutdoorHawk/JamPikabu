using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class ResetGrapplingHookMovementSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _hooks;

        public ResetGrapplingHookMovementSystem(GameContext gameContext)
        {
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook
                ));
        }

        public void Cleanup()
        {
            foreach (var hook in _hooks)
            {
                hook.isXAxisMovementAvailable = true;
                hook.isAscentAvailable = true;
                hook.isDescentAvailable = true;
            }
        }
    }
}