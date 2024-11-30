using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class SetHookXMovementDirectionByInputSystem : IExecuteSystem
    {
        private readonly IGroup<InputEntity> _entities;

        public SetHookXMovementDirectionByInputSystem(InputContext context)
        {
            _entities = context.GetGroup(InputMatcher
                .AllOf(InputMatcher.MovementAxis
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities)
            {
            }
        }
    }
}