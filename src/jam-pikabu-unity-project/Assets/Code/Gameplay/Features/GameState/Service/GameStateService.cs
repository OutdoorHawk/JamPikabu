using Code.Gameplay.Features.GameState.Factory;
using Code.Infrastructure.States.StateMachine;

namespace Code.Gameplay.Features.GameState.Service
{
    public class GameStateService : IGameStateService
    {
        private readonly IGameStateFactory _gameStateFactory;
        private readonly IGameStateMachine _gameStateMachine;

        public GameStateService(IGameStateFactory gameStateFactory, IGameStateMachine gameStateMachine)
        {
            _gameStateFactory = gameStateFactory;
            _gameStateMachine = gameStateMachine;
        }

        public void AskToSwitchState(GameStateTypeId newState)
        {
            _gameStateFactory.CreateSwitchGameStateRequest(newState);
        }

        public void CompleteStateSwitch(GameStateTypeId newState)
        {
            switch (newState)
            {
                case GameStateTypeId.Unknown:
                    break;
                case GameStateTypeId.BeginDay:
                    break;
                case GameStateTypeId.RoundPreparation:
                    break;
                case GameStateTypeId.RoundLoop:
                    break;
                case GameStateTypeId.RoundCompletion:
                    break;
                case GameStateTypeId.EndDay:
                    break;
            }
        }
    }
}