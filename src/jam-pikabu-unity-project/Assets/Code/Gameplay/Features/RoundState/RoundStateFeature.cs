using Code.Gameplay.Features.RoundState.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.RoundState
{
    public sealed class RoundStateFeature : Feature
    {
        public RoundStateFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitRoundStateSystem>());
            
            Add(systems.Create<RequestRoundStartByInputSystem>());
            
            Add(systems.Create<BlockRoundStartAvailableWhenRoundIsProcessingSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenLootIsApplyingSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenInsufficientFundsSystem>());
            
            Add(systems.Create<ProcessRoundStartRequestSystem>());
            Add(systems.Create<ProcessRoundTimerSystem>());
            Add(systems.Create<ProcessRoundOverWhenTimerDoneSystem>());
            
            Add(systems.Create<RefreshRoundCostSystem>());
            
            Add(systems.Create<ResetRoundStartAvailableSystem>());
        }
    }
}