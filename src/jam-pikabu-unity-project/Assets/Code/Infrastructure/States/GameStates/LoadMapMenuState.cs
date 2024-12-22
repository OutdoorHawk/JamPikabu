using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadMapMenuState : SimpleState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IWindowService _windowService;
        private readonly IGameStateMachine _gameStateMachine;

        [Inject]
        public LoadMapMenuState
        (
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWindowService windowService
        )
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _windowService = windowService;
        }

        public override void Enter()
        {
            base.Enter();
            Cursor.lockState = CursorLockMode.None;
            _sceneLoader.LoadScene(SceneTypeId.MapMenu, onLoaded: OnLoaded);
        }

        private void OnLoaded()
        {
            _windowService.ClearUIRoot();

            _gameStateMachine.Enter<MapMenuState>();
        }
    }
}