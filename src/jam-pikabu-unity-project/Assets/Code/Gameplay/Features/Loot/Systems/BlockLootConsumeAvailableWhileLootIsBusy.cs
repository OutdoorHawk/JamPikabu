using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class BlockLootConsumeAvailableWhileLootIsBusy : IExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<GameEntity> _collectedLoot;
        private readonly IGroup<GameEntity> _lootEffectApplier;

        public BlockLootConsumeAvailableWhileLootIsBusy(GameContext context)
        {
            _lootEffectApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));
            
            _collectedLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected
                ));
        }

        public void Execute()
        {
            foreach (var applier in _lootEffectApplier)
            foreach (var loot in _collectedLoot)
            {
                if (loot.isBusy)
                {
                    applier.isLootConsumeAvailable = false;
                    break;
                }

                if (loot.isConsumed)
                {
                    applier.isLootConsumeAvailable = false;
                    break;
                }
            }
        }

        public void Cleanup()
        {
            foreach (var applier in _lootEffectApplier)
            {
                applier.isLootConsumeAvailable = true;
            }
        }
    }
}