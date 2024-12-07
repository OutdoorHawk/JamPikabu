using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class GrapplingHookVisualsSystem : IExecuteSystem, ITearDownSystem
    {
        private readonly IGroup<GameEntity> _hooks;

        public GrapplingHookVisualsSystem(GameContext gameContext)
        {
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.XAxisMoveDirection,
                    GameMatcher.Rigidbody2D,
                    GameMatcher.GrapplingHookBehaviour
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks)
            {
                if (hook.isXAxisMovementAvailable) 
                    hook.GrapplingHookBehaviour.SetupXMovement(hook.XAxisMoveDirection);
                else
                    hook.GrapplingHookBehaviour.SetupXMovement(0);
            }
        }

        public void TearDown()
        {
            foreach (var hook in _hooks)
            {
                hook.GrapplingHookBehaviour.SetupXMovement(0);
            }
        }
    }
}