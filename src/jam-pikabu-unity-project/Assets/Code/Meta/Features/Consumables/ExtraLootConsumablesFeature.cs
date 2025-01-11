using Code.Infrastructure.Systems;
using Code.Meta.Features.Consumables.Systems;

namespace Code.Meta.Features.Consumables
{
    public sealed class ExtraLootConsumablesFeature : Feature
    {
        public ExtraLootConsumablesFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitActiveExtraLootSystem>());
            
            Add(systems.Create<ClearExpiredActiveExtraLootSystem>());
        }
    }
}