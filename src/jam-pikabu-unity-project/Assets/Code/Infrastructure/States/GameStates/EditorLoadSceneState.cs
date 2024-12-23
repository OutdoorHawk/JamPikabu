using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days;
using Code.Meta.Features.Days.Service;
using Code.Progress.Provider;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.States.GameStates
{
    public class EditorLoadSceneState : SimpleState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IProgressProvider _progress;
        private readonly IStaticDataService _staticData;
        private readonly IDaysService _daysService;

        public EditorLoadSceneState
        (
            IGameStateMachine gameStateMachine,
            IProgressProvider progress,
            IStaticDataService staticData,
            IDaysService daysService
        )
        {
            _gameStateMachine = gameStateMachine;
            _progress = progress;
            _staticData = staticData;
            _daysService = daysService;
        }

        public override void Enter()
        {
            base.Enter();

            InitCurrentScene();
        }

        private void InitCurrentScene()
        {
            if (SceneManager.GetActiveScene().name is nameof(SceneTypeId.BootstrapScene))
            {
                _gameStateMachine.Enter<LoadMapMenuState>();
                return;
            }

            if (SceneManager.GetActiveScene().name is nameof(SceneTypeId.MapMenu))
            {
                _gameStateMachine.Enter<LoadMapMenuState>();
            }
            else
            {
                DayProgressData dayProgressData = _daysService.GetDaysProgress().FindLast(x => _daysService.GetDayData(x.DayId).IsBossDay == false);
                _daysService.SetActiveDay(dayProgressData.DayId);
                var parameters = new LoadLevelPayloadParameters()
                {
                    InstantLoad = true
                };
                _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(parameters);
            }
        }
    }
}