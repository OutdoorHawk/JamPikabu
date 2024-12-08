using Code.Common.Destruct;
using Code.Gameplay.Features.Cooldowns.Systems;
using Code.Gameplay.Features.Currency.Systems;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GameState.Systems;
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
            
            Add(systems.Create<CooldownSystem>());
            
            Add(systems.Create<GameStateFeature>());
            
            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}