using Code.Infrastructure.Systems;
using Code.Meta.Features.Storage.Systems;

namespace Code.Meta.Features.Storage
{
    public sealed class StorageFeature : Feature
    {
        public StorageFeature(ISystemFactory systems)
        {
            Add(systems.Create<ProcessAddGoldRequestSystem>());
            
            Add(systems.Create<RefreshGoldSystem>());
        }
    }
}