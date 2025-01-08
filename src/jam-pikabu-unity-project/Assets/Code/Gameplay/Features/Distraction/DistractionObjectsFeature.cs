using Code.Gameplay.Features.Distraction.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Distraction
{
    public sealed class DistractionObjectsFeature : Feature
    {
        public DistractionObjectsFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitDistractionObjectsSystem>());
        }
    }
}