using Code.Gameplay.Windows.Service;
using Code.Infrastructure.SceneLoading;
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

        [Inject]
        public LoadLevelState
        (
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWindowService windowService
        )
        {
            _gameStateMachine = gameStateMachine;
            _windowService = windowService;
            _sceneLoader = sceneLoader;
        }

        public override void Enter(LoadLevelPayloadParameters payload)
        {
            base.Enter(payload);

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
            _gameStateMachine.Enter<BattleEnterState>();
        }

        private async UniTask InitUIAsync()
        {
            _windowService.ClearUIRoot();
        }
    }
}