using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class CreateLootApplierOnRoundOverSystem : ReactiveSystem<GameEntity>
    {
        public CreateLootApplierOnRoundOverSystem(GameContext context) : base(context)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.RoundStateController,
                GameMatcher.RoundOver).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            CreateGameEntity
                .Empty()
                .With(x => x.isLootEffectsApplier = true)
                ;
        }
    }
}