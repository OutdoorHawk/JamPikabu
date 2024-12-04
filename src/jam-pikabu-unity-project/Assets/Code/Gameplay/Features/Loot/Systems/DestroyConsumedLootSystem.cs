using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class DestroyConsumedLootSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _loot;
        private readonly IGroup<GameEntity> _lootApplier;

        public DestroyConsumedLootSystem(GameContext context)
        {
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));
            
            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected
                ));
        }

        public void Cleanup()
        {
            /*foreach (var loot in _loot)
            {
                //loot.isDestructed = true;
            }

            if (_loot.GetEntities().Length == 0)
            {
                foreach (var applier in _lootApplier)
                {
                    applier.isDestructed = true;
                }
            }*/
        }
    }
}