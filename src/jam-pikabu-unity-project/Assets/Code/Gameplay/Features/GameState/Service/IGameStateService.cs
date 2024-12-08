namespace Code.Gameplay.Features.GameState.Service
{
    public interface IGameStateService
    {
        void AskToSwitchState(GameStateTypeId newState);
        void CompleteStateSwitch(GameStateTypeId newState);
    }
}