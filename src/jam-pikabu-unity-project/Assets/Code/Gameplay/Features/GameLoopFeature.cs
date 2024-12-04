using Code.Common.Destruct;
using Code.Gameplay.Features.CollidingView;
using Code.Gameplay.Features.Cooldowns.Systems;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.GameOver;
using Code.Gameplay.Features.GrapplingHook;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.RoundState;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class GameLoopFeature : Feature
    {
        public GameLoopFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindViewFeature>());

            Add(systems.Create<RoundStateFeature>());
            
            Add(systems.Create<LootFeature>());
            
            Add(systems.Create<CurrencyFeature>());
            
            //Add(systems.Create<GameOverFeature>());
        }
    }

    public sealed class GameLoopPhysicsFeature : Feature
    {
        public GameLoopPhysicsFeature(ISystemFactory systems)
        {
            Add(systems.Create<CollidingViewFeature>());
            Add(systems.Create<CooldownSystem>());

            Add(systems.Create<GrapplingHookPhysicsFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}