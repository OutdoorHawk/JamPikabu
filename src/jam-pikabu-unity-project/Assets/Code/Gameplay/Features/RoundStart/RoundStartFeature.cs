using Code.Gameplay.Features.RoundStart.Systems;
using Code.Gameplay.Features.RoundState.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.RoundStart
{
    public sealed class RoundStartFeature : Feature
    {
        public RoundStartFeature(ISystemFactory systems)
        {
            Add(systems.Create<RequestRoundStartByInputSystem>());
            Add(systems.Create<RequestRoundStartByAxisInputSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenRoundIsProcessingSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenRoundNotCompleteSystem>());
            Add(systems.Create<BlockRoundStartWhenAnyOtherWindowOpenSystem>());
            Add(systems.Create<BlockRoundStartWhenOrderWindowNotSeenOpenSystem>());
            Add(systems.Create<BlockRoundStartAvailableWhenNotInRoundPreparationSystem>());

            Add(systems.Create<ProcessRoundStartRequestSystem>());
            Add(systems.Create<ResetRoundStartAvailableSystem>());
        }
    }
}