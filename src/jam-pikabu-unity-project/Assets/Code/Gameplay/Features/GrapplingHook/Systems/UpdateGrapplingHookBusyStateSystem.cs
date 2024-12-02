using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class UpdateGrapplingHookBusyStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hook;

        public UpdateGrapplingHookBusyStateSystem(GameContext context)
        {
            _hook = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.GrapplingHook
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hook)
            {
                if (hook.isAscending)
                {
                    hook.isBusy = true;
                    continue;
                }

                if (hook.isDescending)
                {
                    hook.isBusy = true;
                    continue;
                }

                if (hook.isCollectingLoot)
                {
                    hook.isBusy = true;
                    continue;
                }
                
                if (hook.isClosingClaws)
                {
                    hook.isBusy = true;
                    continue;
                }
                
                hook.isBusy = false;
            }
        }
    }
}