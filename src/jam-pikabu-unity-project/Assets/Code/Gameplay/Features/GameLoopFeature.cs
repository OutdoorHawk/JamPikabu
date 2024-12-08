using Code.Common.Destruct;
using Code.Gameplay.Features.Cooldowns.Systems;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Systems;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GrapplingHook;
using Code.Gameplay.Features.GrapplingHook.Systems;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Systems;
using Code.Gameplay.Features.LootSpawning;
using Code.Gameplay.Features.LootSpawning.Systems;
using Code.Gameplay.Features.Orders;
using Code.Gameplay.Features.Orders.Systems;
using Code.Gameplay.Features.RoundStart;
using Code.Gameplay.Features.RoundState;
using Code.Gameplay.Features.RoundState.Systems;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class GameLoopFeature : Feature
    {
        public GameLoopFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindViewFeature>());
            Add(systems.Create<CooldownSystem>());
            
            Add(systems.Create<InitGrapplingHookSystem>());
            Add(systems.Create<InitGameplayCurrency>());

            Add(systems.Create<RoundStartFeature>());
            Add(systems.Create<ActiveRoundProcessingFeature>());

            Add(systems.Create<LootSpawningFeature>());
            Add(systems.Create<LootPickupSystem>());
            Add(systems.Create<LootConsumeFeature>());
            
            Add(systems.Create<OrderCompletionFeature>());

            Add(systems.Create<CurrencyFeature>());

            Add(systems.Create<GameStateFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }

    public sealed class GameLoopPhysicsFeature : Feature
    {
        public GameLoopPhysicsFeature(ISystemFactory systems)
        {
            Add(systems.Create<GrapplingHookPhysicsFeature>());
        }
    }
}