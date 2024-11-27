using Code.Common.Logger.Service;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        private IGameStateMachine _gameStateMachine;
        private ILoggerService _loggerService;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine, ILoggerService loggerService)
        {
            _loggerService = loggerService;
            _gameStateMachine = gameStateMachine;
        }

        private void Start()
        {
            _loggerService.Log($"URL: {Application.absoluteURL}");
            _loggerService.Log($"QualitySettings: {QualitySettings.GetQualityLevel()}");
            foreach (string settingName in QualitySettings.names) 
                _loggerService.Log($"QualitySettings: {settingName}");
          
            Application.targetFrameRate = 60;
            UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
            _gameStateMachine.Enter<BootstrapState>();
            DontDestroyOnLoad(gameObject);
        }
    }
}