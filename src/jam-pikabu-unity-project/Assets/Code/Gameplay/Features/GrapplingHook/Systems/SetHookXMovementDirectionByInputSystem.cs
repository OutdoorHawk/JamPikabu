using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class SetHookXMovementDirectionByInputSystem : IExecuteSystem
    {
        private readonly IGroup<InputEntity> _input;
        private readonly IGroup<GameEntity> _hooks;

        public SetHookXMovementDirectionByInputSystem(GameContext gameContext, InputContext context)
        {
            _input = context.GetGroup(InputMatcher
                .AllOf(InputMatcher.Input
                ));

            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook
                ));
        }

        public void Execute()
        {
            foreach (var inputEntity in _input)
            foreach (var hook in _hooks)
            {
                if (inputEntity.hasMovementAxis)
                    hook.ReplaceXAxisMoveDirection(inputEntity.MovementAxis.x);
                else
                    hook.ReplaceXAxisMoveDirection(0);
            }
        }
    }
}