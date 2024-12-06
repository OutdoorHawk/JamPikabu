using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadLevelState : SimplePayloadState<LoadLevelPayloadParameters>
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IWindowService _windowService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGameStateHandlerService _gameStateHandler;
        private readonly IUIFactory _uiFactory;

        [Inject]
        public LoadLevelState
        (
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWindowService windowService,
            IGameStateHandlerService gameStateHandlerService,
            IUIFactory uiFactory
        )
        {
            _uiFactory = uiFactory;
            _gameStateHandler = gameStateHandlerService;
            _gameStateMachine = gameStateMachine;
            _windowService = windowService;
            _sceneLoader = sceneLoader;
        }

        public override void Enter(LoadLevelPayloadParameters payload)
        {
            base.Enter(payload);
            
            _gameStateHandler.OnEnterLoadLevel();

            if (payload.InstantLoad)
            {
                OnLoaded();
                return;
            }

            _sceneLoader.LoadScene(payload.LevelName, onLoaded: OnLoaded);
        }

        private void OnLoaded()
        {
            InitAsync().Forget();
        }

        private async UniTaskVoid InitAsync()
        {
            await InitUIAsync();
            
            _gameStateHandler.OnExitLoadLevel();
            _gameStateMachine.Enter<GameEnterState>();
        }

        private async UniTask InitUIAsync()
        {
            _uiFactory.InitializeCamera();
            _windowService.ClearUIRoot();
            _windowService.OpenWindow(WindowTypeId.PlayerHUD);
        }
    }
}