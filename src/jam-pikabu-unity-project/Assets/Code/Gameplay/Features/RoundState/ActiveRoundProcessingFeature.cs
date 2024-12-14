using Code.Gameplay.Features.RoundState.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.RoundState
{
    public sealed class ActiveRoundProcessingFeature : Feature
    {
        public ActiveRoundProcessingFeature(ISystemFactory systems)
        {
            Add(systems.Create<ProcessRoundTimerSystem>());
            Add(systems.Create<MoveToRoundCompleteStateWhenHookAreNotBusySystem>()); ;
            
        }
    }
}