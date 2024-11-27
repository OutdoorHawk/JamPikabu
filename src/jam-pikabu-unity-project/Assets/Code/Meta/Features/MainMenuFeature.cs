using Code.Common.Destruct;
using Code.Infrastructure.Systems;

namespace Code.Meta.Features
{
    public sealed class MainMenuFeature : Feature
    {
        public MainMenuFeature(ISystemFactory systems)
        {
            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}