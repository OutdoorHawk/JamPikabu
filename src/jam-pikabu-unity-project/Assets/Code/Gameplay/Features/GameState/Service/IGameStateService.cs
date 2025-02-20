using System;

namespace Code.Gameplay.Features.GameState.Service
{
    public interface IGameStateService
    {
        GameStateTypeId CurrentState { get; }
        event Action OnStateSwitched;
        void AskToSwitchState(GameStateTypeId newState);
        void CompleteStateSwitch(GameStateTypeId newState);
    }
}