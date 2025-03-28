﻿using Code.Common.Destruct;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.CharacterStats;
using Code.Gameplay.Features.Consumables;
using Code.Gameplay.Features.Cooldowns.Systems;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Days.Systems;
using Code.Gameplay.Features.Distraction;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GrapplingHook;
using Code.Gameplay.Features.GrapplingHook.Systems;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.LootSpawning;
using Code.Gameplay.Features.Orders;
using Code.Gameplay.Features.RoundStart;
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
            Add(systems.Create<CooldownSystem>());

            Add(systems.Create<InitGrapplingHookSystem>());

            Add(systems.Create<CurrencyFeature>());

            Add(systems.Create<RoundStartFeature>());
            Add(systems.Create<ActiveRoundProcessingFeature>());

            Add(systems.Create<GrapplingHookFeature>());

            Add(systems.Create<AbilityFeature>());
            Add(systems.Create<DistractionObjectsFeature>());

            Add(systems.Create<LootSpawningFeature>());
            Add(systems.Create<LootConsumeFeature>());

            Add(systems.Create<ConsumablesFeature>());

            Add(systems.Create<OrdersFeature>());

            Add(systems.Create<StatsFeature>());

            Add(systems.Create<GameStateFeature>());

            Add(systems.Create<ApplyDayProgressOnEndDay>());

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