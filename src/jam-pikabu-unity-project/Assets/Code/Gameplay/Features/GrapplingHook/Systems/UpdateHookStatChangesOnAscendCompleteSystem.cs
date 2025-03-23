using System.Collections.Generic;
using Code.Gameplay.Common.EntityIndices;
using Code.Gameplay.Features.CharacterStats;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class UpdateHookStatChangesOnAscendCompleteSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public UpdateHookStatChangesOnAscendCompleteSystem(GameContext context) : base(context)
        {
            _context = context;
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
            foreach (var entity in entities)
            {
                ReduceAbilityAttempts(entity);
            }
        }

        private void ReduceAbilityAttempts(GameEntity hook)
        {
            foreach (var scaleStat in _context.TargetStatChanges(Stats.Scale, hook.Id))
            {
                if (scaleStat.hasAbilityDuration)
                    scaleStat.ReplaceAbilityDuration(scaleStat.AbilityDuration - 1);
            }
        }
    }
}