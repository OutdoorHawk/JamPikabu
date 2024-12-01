using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class RequestRoundStartByInputSystem : ReactiveSystem<InputEntity>, ICleanupSystem
    {
        private readonly IGroup<GameEntity> _roundControllers;

        public RequestRoundStartByInputSystem(InputContext context, GameContext gameContext) : base(context)
        {
            _roundControllers = gameContext
                .GetGroup(GameMatcher
                    .AllOf(GameMatcher.RoundStateController));
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(InputMatcher.Enter.Removed());
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

        public void Cleanup()
        {
            foreach (var controller in _roundControllers)
            {
                controller.isRoundStartRequest = false;
            }
        }
    }
}