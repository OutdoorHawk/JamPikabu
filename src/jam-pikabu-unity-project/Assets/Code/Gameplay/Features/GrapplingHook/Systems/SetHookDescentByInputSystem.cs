using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class SetHookDescentByInputSystem : ReactiveSystem<InputEntity>
    {
        private readonly IGroup<GameEntity> _hooks;

        public SetHookDescentByInputSystem(GameContext gameContext, InputContext context) : base(context)
        {
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook
                ));
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(InputMatcher.Jump.Added());
        }

        protected override bool Filter(InputEntity entity)
        {
            return true;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            foreach (var entity in entities)
            foreach (var hook in _hooks)
            {
                hook.isDescentRequested = true;
            }
        }
    }
}