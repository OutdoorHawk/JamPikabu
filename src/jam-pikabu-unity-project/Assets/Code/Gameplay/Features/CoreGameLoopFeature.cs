using Code.Common.Destruct;
using Code.Gameplay.Features.Cooldowns.Systems;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.GrapplingHook;
using Code.Gameplay.Features.Loot.Systems;
using Code.Gameplay.Features.LootSpawning.Systems;
using Code.Gameplay.Features.RoundState;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class CoreGameLoopFeature : Feature
    {
        public CoreGameLoopFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindViewFeature>());
            Add(systems.Create<CooldownSystem>());

            Add(systems.Create<ActiveRoundProcessingFeature>());

            Add(systems.Create<ProcessLootPickupSystem>());
            Add(systems.Create<ContinuousSpawnLootSystem>());

            Add(systems.Create<CurrencyFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }

    public sealed class CoreGameLoopPhysicsFeature : Feature
    {
        public CoreGameLoopPhysicsFeature(ISystemFactory systems)
        {

            Add(systems.Create<GrapplingHookPhysicsFeature>());
        }
    }
}