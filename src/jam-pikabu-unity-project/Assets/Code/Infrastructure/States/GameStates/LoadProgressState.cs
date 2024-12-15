using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Gameplay.Tutorial.Service;
using Code.Infrastructure.Localization;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Code.Meta.Features;
using Code.Meta.Features.Days.Configs;
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
            ActualizeProgress();
            LoadNextState();
        }

        private void InitializeProgress()
        {
            if (_saveLoadService.HasSavedProgress)
                _saveLoadService.LoadProgress();
            else
                CreateNewProgress();
        }

        private void ActualizeProgress()
        {
            ActualizeProgressFeature loadProgressFeature = _systemFactory.Create<ActualizeProgressFeature>();
            loadProgressFeature.Initialize();
            loadProgressFeature.Execute();

            loadProgressFeature.DeactivateReactiveSystems();
            loadProgressFeature.ClearReactiveSystems();

            loadProgressFeature.Cleanup();
            loadProgressFeature.TearDown();
        }

        private void CreateNewProgress()
        {
            _saveLoadService.CreateProgress();

            CreateStorages();
        }

        private void CreateStorages()
        {
            int startGoldAmount = _staticData.GetStaticData<DaysStaticData>().StartGoldAmount;

            CreateMetaEntity
                .Empty()
                .With(x => x.isStorage = true)
                .With(x => x.AddGold(startGoldAmount));
        }

        private void LoadNextState()
        {
            _gameStateHandlerService.OnExitLoadProgressState();

#if UNITY_EDITOR
            _stateMachine.Enter<EditorLoadSceneState>();
            return;
#endif
            _stateMachine.Enter<LoadLevelSimpleState, LoadLevelPayloadParameters>(new LoadLevelPayloadParameters());
        }
    }
}