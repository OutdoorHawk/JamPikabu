using Code.Infrastructure.Systems;
using Code.Infrastructure.View.Systems;

namespace Code.Infrastructure.View
{
    public sealed class BindViewFeature : Feature
    {
        public BindViewFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindEntityViewFromResourcesPathSystem>());
            Add(systems.Create<BindEntityViewFromPrefabSystem>());
            Add(systems.Create<BindEntityViewFromAddressablesPathSystem>());
            Add(systems.Create<SetupParentForViewSystem>());
        }
    }
}