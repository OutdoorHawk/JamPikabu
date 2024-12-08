using Code.Gameplay.Features.Loot.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Loot
{
    public sealed class LootConsumeFeature : Feature
    {
        public LootConsumeFeature(ISystemFactory systems)
        {
            Add(systems.Create<ConsumeLootValueSystem>());
            Add(systems.Create<ConsumeLootVisualsSystem>());
            
            Add(systems.Create<DestroyConsumedLootSystem>());
        }
    }
}