using System;
using System.Collections.Generic;
using System.Threading;
using Code.Common.Logger.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Tutorial.Config;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Progress.Data.Tutorial;
using Code.Progress.Provider;
using Code.Progress.SaveLoadService;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Tutorial.Service
{
    public class TutorialService : ITutorialService,
        IEnterMainMenuStateHandler,
        IEnterGameLoopStateHandler,
        IExitGameLoopStateHandler
    {
        private readonly List<ITutorialProcessor> _processors;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IStaticDataService _staticData;
        private readonly IProgressProvider _provider;
        private readonly ILoggerService _logger;
        private readonly IWindowService _windowService;

        private readonly Dictionary<TutorialTypeId, TutorialUserData> _tutorialUserData = new();
        private readonly Dictionary<TutorialTypeId, ITutorialProcessor> _tutorialProcessors = new();

        private readonly List<TutorialConfig> _configs = new();

        private (ITutorialProcessor processor, CancellationTokenSource source) _activeProcessor;

        public event Action OnTutorialUpdate;

        public OrderType OrderType => OrderType.Last;

        public TutorialService
        (
            List<ITutorialProcessor> processors,
            IStaticDataService staticDataService,
            ISaveLoadService saveLoadService,
            IProgressProvider provider,
            ILoggerService logger,
            IWindowService windowService
        )
        {
            _saveLoadService = saveLoadService;
            _processors = processors;
            _staticData = staticDataService;
            _provider = provider;
            _logger = logger;
            _windowService = windowService;
        }

        public void Initialize()
        {
            InitData();
        }

        public void OnEnterMainMenu()
        {
            TryStartTutorial();
        }

        public void OnEnterGameLoop()
        {
            TryStartTutorial();
        }

        public void OnExitGameLoop()
        {
            SkipCurrentTutorial();
        }

        public void SkipCurrentTutorial()
        {
            ResetTutorialToken();
        }

        public bool IsTutorialStartedOrCompleted(TutorialTypeId type)
        {
            if (_tutorialUserData.TryGetValue(type, out var userData) == false)
                return false;

            if (userData.Completed)
                return true;

            if (_activeProcessor.processor != null && _activeProcessor.processor.TypeId == type)
                return true;

            return false;
        }

        private void InitData()
        {
            List<TutorialUserData> userDatas = _provider.Progress.Tutorial.TutorialUserDatas;

            _tutorialUserData.Clear();
            _tutorialProcessors.Clear();
            _configs.Clear();

            foreach (var userData in userDatas)
                _tutorialUserData.Add(userData.Type, userData);

            foreach (var processor in _processors)
                _tutorialProcessors.Add(processor.TypeId, processor);

            List<TutorialConfig> tutorialsConfigs = _staticData.GetStaticData<TutorialStaticData>().Configs;

            _configs.AddRange(tutorialsConfigs);
            _configs.Sort((x, y) => x.Order.CompareTo(y.Order));
        }

        private void TryStartTutorial()
        {
            foreach (TutorialConfig config in _configs)
            {
                if (CheckCanStartTutorial(config) == false)
                    continue;

                StartTutorial(config).Forget();
                break;
            }

            OnTutorialUpdate?.Invoke();
        }

        private bool CheckCanStartTutorial(TutorialConfig config)
        {
            if (config.DisableTutorial)
                return false;

            TutorialTypeId typeId = config.Type;

            if (_tutorialUserData.TryGetValue(typeId, out TutorialUserData savedData))
            {
                if (savedData.Completed)
                    return false;
            }
            else
            {
                savedData = CreateNewTutorialUserData(typeId);
            }

            if (_tutorialProcessors.TryGetValue(typeId, out ITutorialProcessor processor) == false)
            {
                _logger.LogError($"<b><color=cyan>[Tutorial]</b></color> processor for type: {typeId} not found! Tutorial will be skipped.");
                return false;
            }

            if (processor.CanSkipTutorial())
            {
                MarkTutorialCompleted(savedData);
                return false;
            }
            
            if (processor.CheckLevelsPassedNeeds() == false)
                return false;

            if (processor.CanStartTutorial() == false)
                return false;

            return true;
        }

        private async UniTaskVoid StartTutorial(TutorialConfig config)
        {
            TutorialTypeId tutorialTypeId = config.Type;
            ITutorialProcessor tutorialProcessor = _tutorialProcessors[tutorialTypeId];
            TutorialUserData tutorialSaveData = _tutorialUserData[tutorialTypeId];

            ResetTutorialToken();
            _activeProcessor.processor = tutorialProcessor;

            _logger.Log($"<b><color=cyan>[Tutorial]</b></color> Start step {tutorialTypeId}");

            try
            {
                await tutorialProcessor.Process(_activeProcessor.source.Token);
            }
            catch (Exception e)
            {
                if (Application.isPlaying == false)
                    return;
                
                _logger.LogError($"<b><color=cyan>[Tutorial]</b></color> Exception: {e}\n" +
                                 "Tutorial will be skipped.");
            }

            _windowService.Close(WindowTypeId.Tutorial);
            _activeProcessor.source?.Cancel();
            tutorialProcessor.Finalization();

            MarkTutorialCompleted(tutorialSaveData);
            TryStartTutorial();
        }

        private TutorialUserData CreateNewTutorialUserData(TutorialTypeId typeId)
        {
            TutorialUserData newTutorialUserData = new() { TypeInt = (int)typeId };
            _provider.Progress.Tutorial.TutorialUserDatas.Add(newTutorialUserData);
            _tutorialUserData[typeId] = newTutorialUserData;
            _saveLoadService.SaveProgress();
            return newTutorialUserData;
        }

        private void ResetTutorialToken()
        {
            _activeProcessor.source?.Cancel();
            _activeProcessor.source = new CancellationTokenSource();
        }

        private void MarkTutorialCompleted(TutorialUserData tutorialSaveData)
        {
            _logger.Log($"<b><color=cyan>[Tutorial]</b></color> Complete {tutorialSaveData.Type}");
            tutorialSaveData.Completed = true;

            if (_staticData.GetStaticData<TutorialStaticData>().DebugDisableSave) 
                return;
            
            _saveLoadService.SaveProgress();
        }
    }
}