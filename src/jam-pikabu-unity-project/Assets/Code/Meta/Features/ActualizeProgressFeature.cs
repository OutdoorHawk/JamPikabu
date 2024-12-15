using Code.Infrastructure.Systems;
using Code.Meta.Features.Days.Systems;

namespace Code.Meta.Features
{
    public sealed class ActualizeProgressFeature : Feature
    {
        public ActualizeProgressFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeDaySystem>());
        }
    }
}