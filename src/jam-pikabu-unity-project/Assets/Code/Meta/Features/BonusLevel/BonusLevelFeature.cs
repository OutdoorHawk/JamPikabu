using Code.Infrastructure.Systems;
using Code.Meta.Features.BonusLevel.Systems;

namespace Code.Meta.Features.BonusLevel
{
    public sealed class BonusLevelFeature : Feature
    {
        public BonusLevelFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitBonusLevelTimerSystem>());
            Add(systems.Create<ProcessBonusLevelTimerSystem>());
        }
    }
}