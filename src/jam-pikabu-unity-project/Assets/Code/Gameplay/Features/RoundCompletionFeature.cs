using Code.Common.Destruct;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.GameOver;
using Code.Gameplay.Features.GrapplingHook.Systems;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class RoundCompletionFeature : Feature
    {
        public RoundCompletionFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindViewFeature>());

            Add(systems.Create<LootConsumeFeature>());
            Add(systems.Create<OrderCompletionFeature>());
            Add(systems.Create<GameOverFeature>());
            
            Add(systems.Create<CurrencyFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
        }
    }
}