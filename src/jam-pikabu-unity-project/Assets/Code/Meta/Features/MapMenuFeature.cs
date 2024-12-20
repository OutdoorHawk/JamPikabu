using Code.Common.Destruct;
using Code.Infrastructure.Systems;
using Code.Meta.Features.Days.Systems;
using Code.Meta.Features.LootCollection;
using Code.Meta.Features.MapBlocks;
using Code.Meta.Features.Storage;
using Code.Progress.Systems;

namespace Code.Meta.Features
{
    public sealed class MapMenuFeature : Feature
    {
        public MapMenuFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeDaySystem>());
            
            Add(systems.Create<LootCollectionFeature>());
            Add(systems.Create<MapBlocksFeature>());
            
            Add(systems.Create<StorageFeature>());
            
            Add(systems.Create<PeriodicallySaveProgressSystem>(30f));

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}