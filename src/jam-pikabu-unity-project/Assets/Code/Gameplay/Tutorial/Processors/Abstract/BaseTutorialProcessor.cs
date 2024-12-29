using System;
using System.Collections.Generic;
using System.Threading;
using Code.Gameplay.Input.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Tutorial.Config;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Service;
using Code.Progress.Data;
using Code.Progress.Provider;
using Cysharp.Threading.Tasks;
using Entitas;
using Zenject;

namespace Code.Gameplay.Tutorial.Processors.Abstract
{
    public abstract class BaseTutorialProcessor : ITutorialProcessor
    {
        protected IInputService _inputService;
        protected IWindowService _windowService;
        protected IStaticDataService _staticData;
        protected IDaysService _daysService;
        protected MetaContext _metaContext;
        protected GameContext _gameContext;

        private IGameStateMachine _gameStateMachine;
        private IProgressProvider _progressProvider;

        private readonly List<WindowTypeId> _openedWindowsCache = new();

        protected PlayerProgress Progress => _progressProvider.Progress;
        private TutorialStaticData TutorialStaticData => _staticData.Get<TutorialStaticData>();

        [Inject]
        private void Construct
        (
            IInputService inputService,
            IWindowService windowService,
            GameContext gameContext,
            MetaContext metaContext,
            IProgressProvider progressProvider,
            IGameStateMachine gameStateMachine,
            IStaticDataService staticDataService,
            IDaysService daysService
        )

        {
            _daysService = daysService;
            _staticData = staticDataService;
            _progressProvider = progressProvider;
            _gameStateMachine = gameStateMachine;
            _windowService = windowService;
            _gameContext = gameContext;
            _metaContext = metaContext;
            _inputService = inputService;
        }

        public abstract TutorialTypeId TypeId { get; }
        public abstract bool CanStartTutorial();
        public abstract bool CanSkipTutorial();

        async UniTask ITutorialProcessor.Process(CancellationToken token)
        {
            await ProcessInternal(token);
        }

        public bool CheckLevelsPassedNeeds()
        {
            TutorialConfig config = TutorialStaticData.Configs.Find(x => x.Type == TypeId);

            if (config.CompletedLevelsNeedToStart == 0)
                return true;

            if (_daysService.GetDaysProgress().Count < config.CompletedLevelsNeedToStart)
                return false;

            return true;
        }

        protected abstract UniTask ProcessInternal(CancellationToken token);

        public abstract void Finalization();

        protected bool CheckCurrentGameState<T>() where T : IExitableState
        {
            return _gameStateMachine.ActiveState is T;
        }

        protected GameEntity[] GetGameEntitiesGroup(IMatcher<GameEntity> matcher)
        {
            return _gameContext.GetGroup(matcher).GetEntities();
        }

        protected MetaEntity[] GetMetaEntitiesGroup(IMatcher<MetaEntity> matcher)
        {
            return _metaContext.GetGroup(matcher).GetEntities();
        }

        protected TutorialWindow GetCurrentWindow()
        {
            _windowService.TryGetWindow(out TutorialWindow tutorialWindow);
            return tutorialWindow;
        }

        protected async UniTask<T> WaitForWindowToOpen<T>(CancellationToken token, int maxFrames = 240) where T : BaseWindow
        {
            while (maxFrames > 0)
            {
                if (_windowService.TryGetWindow(out T window))
                    return window;

                maxFrames--;
                await UniTask.Yield(token);
            }

            throw new Exception($"Max attempts for waiting {typeof(T)} to open, has been exceeded!");
        }

        protected bool AnyWindowOpen()
        {
            return _windowService.AnyWindowOpen();
        }

        protected async UniTask WaitForAnyWindowOpen(CancellationToken token)
        {
            await WaitForAnyWindowOpen(token, WindowTypeId.MainMenu, WindowTypeId.Tutorial);
        }

        protected async UniTask WaitForAnyWindowOpen(CancellationToken token, params WindowTypeId[] excludeTypes)
        {
            while (token.IsCancellationRequested == false)
            {
                _openedWindowsCache.Clear();

                await UniTask.Yield(token);

                if (_windowService.Windows.Count == 0)
                    continue;

                _openedWindowsCache.AddRange(_windowService.Windows.Keys);

                foreach (WindowTypeId excludeType in excludeTypes)
                    _openedWindowsCache.Remove(excludeType);

                if (_openedWindowsCache.Count != 0)
                    break;
            }

            _openedWindowsCache.Clear();
        }
        
        protected void ResetAll()
        {
            GetCurrentWindow()
                .ClearHighlights()
                .HideMessages()
                .HideArrow()
                .HideDarkBackground();
        }
    }
}