using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Progress.Provider;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.States.GameStates
{
    public class EditorLoadSceneState : SimpleState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IProgressProvider _progress;
        private readonly IStaticDataService _staticData;

        public EditorLoadSceneState
        (
            IGameStateMachine gameStateMachine,
            IProgressProvider progress,
            IStaticDataService staticData
        )
        {
            _gameStateMachine = gameStateMachine;
            _progress = progress;
            _staticData = staticData;
        }

        public override void Enter()
        {
            base.Enter();

            InitCurrentScene();
        }

        private void InitCurrentScene()
        {
            if (SceneManager.GetActiveScene().name is nameof(SceneTypeId.MainMenu) or nameof(SceneTypeId.BootstrapScene))
                _gameStateMachine.Enter<LoadMainMenuState>();
            else
            {
            }
        }
    }
}