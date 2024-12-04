using Code.Gameplay.Features.Loot.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Loot
{
    public sealed class LootFeature : Feature
    {
        public LootFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitLootSpawnSystem>());
            Add(systems.Create<SpawnLootSystem>());
            Add(systems.Create<SetLootInitialSpeedSystem>());
            
            Add(systems.Create<ProcessLootPickup>());
            
            Add(systems.Create<UpdateLootItemUIGoldValue>());
            
            Add(systems.Create<CreateLootApplierOnRoundOverSystem>());
            
            Add(systems.Create<BlockLootEffectApplicationWhileLootIsBusy>());
            Add(systems.Create<BlockLootConsumeAvailableWhileLootIsBusy>());
            
            //Add(systems.Create<ApplyLootIncreaseValueEffectSystem>());
            
            Add(systems.Create<ConsumeLootValueSystem>());
            Add(systems.Create<ConsumeLootVisualsSystem>());
            
            Add(systems.Create<DestroyConsumedLootSystem>());
        }
    }
}