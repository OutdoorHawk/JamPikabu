using Code.Common.Destruct;
using Code.Gameplay.Features.CollidingView;
using Code.Gameplay.Features.Cooldowns.Systems;
using Code.Gameplay.Features.GrapplingHook;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class BattleFeature : Feature
    {
        public BattleFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindViewFeature>());
        }
    }

    public sealed class BattlePhysicsFeature : Feature
    {
        public BattlePhysicsFeature(ISystemFactory systems)
        {
            Add(systems.Create<CollidingViewFeature>());
            Add(systems.Create<CooldownSystem>());

            Add(systems.Create<GrapplingHookFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}