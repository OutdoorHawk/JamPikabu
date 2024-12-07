using Code.Common.Destruct;
using Code.Gameplay.Features.Cooldowns.Systems;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.LootSpawning.Systems;
using Code.Gameplay.Features.Orders.Systems;
using Code.Gameplay.Features.RoundStart;
using Code.Gameplay.Features.RoundState.Systems;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class RoundPreparationLoopFeature : Feature
    {
        public RoundPreparationLoopFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindViewFeature>());
            Add(systems.Create<CooldownSystem>());
            Add(systems.Create<RoundStartFeature>());

            Add(systems.Create<CreateOrderOnRoundStartSystem>());
            Add(systems.Create<ContinuousSpawnLootSystem>());

            Add(systems.Create<CurrencyFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
            Add(systems.Create<ResetRoundStartAvailableSystem>());
        }
    }
}