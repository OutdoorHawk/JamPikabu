using Code.Common.Destruct;
using Code.Gameplay.Features.CollidingView;
using Code.Gameplay.Features.Cooldowns.Systems;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.GrapplingHook;
using Code.Gameplay.Features.Loot;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class BattleFeature : Feature
    {
        public BattleFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindViewFeature>());

            Add(systems.Create<LootFeature>());
            
            Add(systems.Create<CurrencyFeature>());
            
        }
    }

    public sealed class BattlePhysicsFeature : Feature
    {
        public BattlePhysicsFeature(ISystemFactory systems)
        {
            Add(systems.Create<CollidingViewFeature>());
            Add(systems.Create<CooldownSystem>());

            Add(systems.Create<GrapplingHookPhysicsFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}