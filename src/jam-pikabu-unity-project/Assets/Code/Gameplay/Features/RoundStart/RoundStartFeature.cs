using Code.Gameplay.Features.RoundState.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.RoundStart
{
    public sealed class RoundStartFeature : Feature
    {
        public RoundStartFeature(ISystemFactory systems)
        {
            Add(systems.Create<RequestRoundStartByInputSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenRoundIsProcessingSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenRoundNotCompleteSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenInsufficientFundsSystem>());

            Add(systems.Create<ProcessRoundStartRequestSystem>());
            Add(systems.Create<RefreshRoundCostSystem>());
        }
    }
}