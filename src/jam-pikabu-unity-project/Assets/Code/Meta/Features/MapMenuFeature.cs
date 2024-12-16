using Code.Common.Destruct;
using Code.Infrastructure.Systems;
using Code.Meta.Features.Days.Systems;
using Code.Meta.Features.LootCollection;
using Code.Meta.Features.Storage;

namespace Code.Meta.Features
{
    public sealed class MapMenuFeature : Feature
    {
        public MapMenuFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeDaySystem>());
            
            Add(systems.Create<LootCollectionFeature>());
            Add(systems.Create<StorageFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}