using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootEffectsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public ApplyLootEffectsSystem(GameContext context)
        {
            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootEffectsApplier
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