using Code.Common.Destruct;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Orders.Systems;
using Code.Gameplay.Features.RoundState.Systems;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features
{
    public sealed class RoundPreparationLoopFeature : Feature
    {
        public RoundPreparationLoopFeature(ISystemFactory systems)
        {
            // Add(systems.Create<RoundStateFeature>());
            Add(systems.Create<BindViewFeature>());

            Add(systems.Create<RequestRoundStartByInputSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenRoundIsProcessingSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenRoundNotCompleteSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenInsufficientFundsSystem>());

            Add(systems.Create<ProcessRoundStartRequestSystem>());
            Add(systems.Create<RefreshRoundCostSystem>());

            Add(systems.Create<UpdateLootValueForOrderOnRoundStartSystem>());

            Add(systems.Create<CurrencyFeature>());

            Add(systems.Create<ProcessDestructedFeature>());
            Add(systems.Create<ResetRoundStartAvailableSystem>());
        }
    }
}