using System.Collections.Generic;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadLevelSimpleState : SimplePayloadState<LoadLevelPayloadParameters>
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IWindowService _windowService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGameStateHandlerService _gameStateHandler;
        private readonly IUIFactory _uiFactory;
        private readonly IDaysService _daysService;
        private readonly IStaticDataService _staticDataService;

        [Inject]
        public LoadLevelSimpleState
        (
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWindowService windowService,
            IGameStateHandlerService gameStateHandlerService,
            IUIFactory uiFactory,
            IDaysService daysService,
            IStaticDataService staticDataService
        )
        {
            _uiFactory = uiFactory;
            _daysService = daysService;
            _staticDataService = staticDataService;
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
                OnLoaded(payload);
                return;
            }

            foreach (var day in Contexts.sharedInstance.meta.GetGroup(MetaMatcher.Day))
            {
                DayData dayData = GetDayData(day.Day);
            
                _sceneLoader.LoadScene(dayData.SceneId, onLoaded: () => OnLoaded(payload));
            }
        }
        
        public DayData GetDayData(int currentDay)
        {
            List<DayData> dayDatas = _staticDataService.GetStaticData<DaysStaticData>().Days;
            foreach (DayData data in dayDatas)
            {
                if (data.Id >= currentDay)
                    return data;
            }

            return dayDatas[^1];
        }

        private void OnLoaded(LoadLevelPayloadParameters payload)
        {
            InitAsync(payload).Forget();
        }

        private async UniTaskVoid InitAsync(LoadLevelPayloadParameters payload)
        {
            await InitUIAsync();
            
            _gameStateHandler.OnExitLoadLevel();
            _gameStateMachine.Enter<GameEnterState>();
            payload.LoadCallback?.Invoke();
        }

        private async UniTask InitUIAsync()
        {
            _uiFactory.InitializeCamera();
            _windowService.ClearUIRoot();
            _windowService.OpenWindow(WindowTypeId.PlayerHUD);
        }
    }
}