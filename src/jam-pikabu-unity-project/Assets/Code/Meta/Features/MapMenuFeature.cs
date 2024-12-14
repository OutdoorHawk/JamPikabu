using Code.Common.Destruct;
using Code.Infrastructure.Systems;

namespace Code.Meta.Features
{
    public sealed class MapMenuFeature : Feature
    {
        public MapMenuFeature(ISystemFactory systems)
        {
            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}