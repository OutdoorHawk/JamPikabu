using System.Collections.Generic;
using Code.Gameplay.Common.EntityIndices;
using Code.Gameplay.Features.CharacterStats;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class UpdateStickyToHookOnAscendCompleteSystem : ReactiveSystem<GameEntity>
    {
        private readonly IGroup<GameEntity> _stickyObjects;
        private readonly List<GameEntity> _bufferSticky = new(8);

        public UpdateStickyToHookOnAscendCompleteSystem(GameContext context) : base(context)
        {
            _stickyObjects = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.StickToKinematic,
                    GameMatcher.Target,
                    GameMatcher.View
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.Ascending)
                .Removed());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isGrapplingHook && entity.isAscending == false;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var hook in entities)
            foreach (var sticky in _stickyObjects.GetEntities(_bufferSticky))
            {
                if (sticky.Target == hook.Id) 
                    sticky.StickToKinematic.ReduceDuration(1);
            }
        }
    }
}