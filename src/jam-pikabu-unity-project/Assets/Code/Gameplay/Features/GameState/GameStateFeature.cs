using Code.Gameplay.Features.GameState.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.GameState
{
    public sealed class GameStateFeature : Feature
    {
        public GameStateFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeGameStateSystem>());
            
            Add(systems.Create<ProcessBeginDayStateSystem>());
            Add(systems.Create<ProcessRoundPreparationStateSystem>());
            Add(systems.Create<ProcessRoundLoopStateSystem>());
            Add(systems.Create<ProcessRoundCompletionStateSystem>());
            Add(systems.Create<ProcessEndDayStateSystem>());
            
            Add(systems.Create<ProcessGameStateSwitchToRoundPreparationSystem>());
            Add(systems.Create<ProcessGameStateSwitchToRoundLoopSystem>());
            Add(systems.Create<ProcessGameStateSwitchToRoundCompletionSystem>());
            Add(systems.Create<ProcessGameStateSwitchToEndDaySystem>());
            Add(systems.Create<ProcessGameStateSwitchToBeginDaySystem>());
        }
    }
}