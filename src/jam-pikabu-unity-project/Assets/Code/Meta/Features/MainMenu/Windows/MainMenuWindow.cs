using Code.Gameplay.Windows;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Zenject;

namespace Code.Meta.Features.MainMenu.Windows
{
    public class MainMenuWindow : BaseWindow
    {
        private IGameStateMachine _gameStateMachine;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }
        
        public void LoadCurrentLevel()
        {
            _gameStateMachine.Enter<LoadLevelSimpleState, LoadLevelPayloadParameters>(default);
        }
    }
}