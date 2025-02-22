using Code.Gameplay.Features.Loot.Systems;
using Code.Gameplay.Features.Loot.Systems.Code.Gameplay.Features.Loot.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Loot
{
    public sealed class LootConsumeFeature : Feature
    {
        public LootConsumeFeature(ISystemFactory systems)
        {
            Add(systems.Create<LootIngredientPickupLogicSystem>());
            
            Add(systems.Create<GoldLootPickupSystem>());
            Add(systems.Create<WoodChipLootPickupSystem>());
         
            Add(systems.Create<ConsumableLootPickupSystem>());
            
            Add(systems.Create<LootIngredientPickupVisualSystem>());
            
            Add(systems.Create<ConsumeLootValueSystem>());
            Add(systems.Create<ConsumeLootVisualsSystem>());
            
            Add(systems.Create<CleanupCollectLootRequestSystem>());
        }
    }
}