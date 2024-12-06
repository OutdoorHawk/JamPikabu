using Code.Common.Destruct;
using Code.Gameplay.Features.Currency.Systems;
using Code.Gameplay.Features.GrapplingHook.Systems;
using Code.Gameplay.Features.LootSpawning;
using Code.Gameplay.Features.Orders.Systems;
using Code.Gameplay.Features.RoundState.Systems;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class BeginDayFeature : Feature
    {
        public BeginDayFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindViewFeature>());

            Add(systems.Create<InitRoundStateSystem>());
            Add(systems.Create<InitDayOrdersSystem>());
            Add(systems.Create<InitGrapplingHookSystem>());
            Add(systems.Create<InitGameplayCurrency>());
            Add(systems.Create<LootSpawningFeature>());
            
            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}