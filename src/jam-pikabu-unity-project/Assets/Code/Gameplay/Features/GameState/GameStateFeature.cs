using Code.Gameplay.Features.GameState.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.GameState
{
    public sealed class GameStateFeature : Feature
    {
        public GameStateFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeGameStateSystem>());

            Add(systems.Create<EnterRoundPreparationGameStateSystem>());
            Add(systems.Create<EnterRoundLoopGameState>());
            Add(systems.Create<EnterRoundCompleteGameStateSystem>());
            Add(systems.Create<EnterEndDayGameStateSystem>());
            Add(systems.Create<EnterBeginDayGameStateSystem>());
            
            Add(systems.Create<ProcessBeginDayStateSystem>());
            Add(systems.Create<ProcessRoundPreparationStateSystem>());
            Add(systems.Create<ProcessRoundLoopStateSystem>());
            Add(systems.Create<ProcessRoundCompletionStateSystem>());
            Add(systems.Create<ProcessEndDayStateSystem>());
        }
    }
}