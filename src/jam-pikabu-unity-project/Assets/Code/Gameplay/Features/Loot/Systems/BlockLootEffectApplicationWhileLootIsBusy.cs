using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class BlockLootEffectApplicationWhileLootIsBusy : IExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<GameEntity> _collectedLoot;
        private readonly IGroup<GameEntity> _lootEffectApplier;

        public BlockLootEffectApplicationWhileLootIsBusy(GameContext context)
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
                    applier.isEffectApplicationAvailable = false;
                    break;
                }
            }
        }

        public void Cleanup()
        {
            foreach (var applier in _lootEffectApplier)
            {
                applier.isEffectApplicationAvailable = true;
            }
        }
    }
}