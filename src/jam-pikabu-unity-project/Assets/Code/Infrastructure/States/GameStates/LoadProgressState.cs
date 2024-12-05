using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Gameplay.Tutorial.Service;
using Code.Infrastructure.Localization;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Code.Progress.Provider;
using Code.Progress.SaveLoadService;
using Zenject;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadProgressState : SimpleState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ISaveLoadService _saveLoadService;
        private readonly ILocalizationService _localizationService;
        private readonly IStaticDataService _staticData;
        private readonly IProgressProvider _progress;
        private readonly LazyInject<ITutorialService> _tutorialService;
        private readonly ISystemFactory _systemFactory;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        public LoadProgressState
        (
            IGameStateMachine stateMachine,
            ISaveLoadService saveLoadService,
            ILocalizationService localizationService,
            IStaticDataService staticData,
            IProgressProvider progress,
            LazyInject<ITutorialService> tutorialService,
            ISystemFactory systemFactory,
            IGameStateHandlerService gameStateHandlerService
        )
        {
            _gameStateHandlerService = gameStateHandlerService;
            _systemFactory = systemFactory;
            _tutorialService = tutorialService;
            _staticData = staticData;
            _progress = progress;
            _saveLoadService = saveLoadService;
            _localizationService = localizationService;
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            _gameStateHandlerService.OnEnterLoadProgressState();
            InitGameProgress();
        }

        private void InitGameProgress()
        {
            InitializeProgress();
            _localizationService.InitLanguageSettings();
            _tutorialService.Value.Initialize();
            LoadNextState();
        }

        private void InitializeProgress()
        {
            if (_saveLoadService.HasSavedProgress)
                _saveLoadService.LoadProgress();
            else
                CreateNewProgress();
        }

        private void CreateNewProgress()
        {
            _saveLoadService.CreateProgress();

            CreateHardStorage();
        }

        private static void CreateHardStorage()
        {
            CreateMetaEntity.Empty()
                .With(x => x.isStorage = true)
                .AddHard(0)
                ;
        }
        
        private void LoadNextState()
        {
            _gameStateHandlerService.OnExitLoadProgressState();
#if UNITY_EDITOR
            _stateMachine.Enter<EditorLoadSceneState>();
#else
            _stateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(new LoadLevelPayloadParameters(levelName: nameof(SceneTypeId.Level_1)));
#endif
        }
        
    }
}