namespace Code.Gameplay.Features.GameState.Factory
{
    public interface IGameStateFactory
    {
        void CreateSwitchGameStateRequest(GameStateTypeId newState);
    }
}