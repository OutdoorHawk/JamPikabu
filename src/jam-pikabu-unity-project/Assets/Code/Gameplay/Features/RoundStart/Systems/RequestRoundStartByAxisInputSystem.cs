using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundStart.Systems
{
    public class RequestRoundStartByAxisInputSystem : ReactiveSystem<InputEntity>
    {
        private readonly IGroup<GameEntity> _roundControllers;

        public RequestRoundStartByAxisInputSystem(InputContext context, GameContext gameContext) : base(context)
        {
            _roundControllers = gameContext
                .GetGroup(GameMatcher
                    .AllOf(GameMatcher.RoundStateController));
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(InputMatcher
                .AnyOf(
                    InputMatcher.MovementAxis).Added());
        }

        protected override bool Filter(InputEntity entity)
        {
            return true;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            foreach (var controller in _roundControllers)
            {
                controller.isRoundStartRequest = true;
            }
        }
    }
}