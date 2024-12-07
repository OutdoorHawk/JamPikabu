using Entitas;

namespace Code.Gameplay.Features.RoundStart.Systems
{
    public class RequestRoundStartByAxisInputSystem : IExecuteSystem
    {
        private readonly IGroup<InputEntity> _entities;
        private readonly IGroup<GameEntity> _roundControllers;

        public RequestRoundStartByAxisInputSystem(InputContext context, GameContext gameContext)
        {
            _entities = context.GetGroup(InputMatcher
                .AllOf(InputMatcher.MovementAxis
                ));
            
            _roundControllers = gameContext
                .GetGroup(GameMatcher
                    .AllOf(GameMatcher.RoundStateController));
        }

        public void Execute()
        {
            foreach (var entity in _entities)
            {
                if (entity.MovementAxis.x > 0)
                {
                    foreach (var controller in _roundControllers)
                    {
                        controller.isRoundStartRequest = true;
                    }
                }
            }
        }
    }
}