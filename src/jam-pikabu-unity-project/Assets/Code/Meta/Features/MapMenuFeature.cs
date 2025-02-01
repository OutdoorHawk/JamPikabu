using Code.Common.Destruct;
using Code.Infrastructure.Systems;
using Code.Meta.Features.BonusLevel;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Days.Systems;
using Code.Meta.Features.ExpirationTimer;
using Code.Meta.Features.LootCollection;
using Code.Meta.Features.MapBlocks;
using Code.Meta.Features.Storage;
using Code.Meta.UI.Shop;
using Code.Progress.Systems;

namespace Code.Meta.Features
{
    public sealed class MapMenuFeature : Feature
    {
        public MapMenuFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeDaySystem>());
            Add(systems.Create<SyncDayStarsSeenSystem>());
            
            Add(systems.Create<ExpirationTimerSystem>(1f));
            
            Add(systems.Create<LootCollectionFeature>());
            Add(systems.Create<MapBlocksFeature>());
            Add(systems.Create<BonusLevelFeature>());
            Add(systems.Create<ExtraLootConsumablesFeature>());
            
            Add(systems.Create<ShopFeature>());
            Add(systems.Create<StorageFeature>());
            
            Add(systems.Create<PeriodicallySaveProgressSystem>(30f));

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}