using System.Collections.Generic;
using System.Threading;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.StaticData;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.RoundState.Service
{
    public class RoundStateService : IRoundStateService
    {
        private readonly IRoundStateFactory _roundStateFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly LazyInject<IGameOverService> _gameOverService;

        private List<DayData> _daysData;
        private int _currentRound = 1;
        private int _currentDay = 1;

        public int CurrentRound => _currentRound;

        public int CurrentDay => _currentDay;

        public RoundStateService
        (
            IRoundStateFactory roundStateFactory,
            IStaticDataService staticDataService,
            IGameStateMachine gameStateMachine,
            LazyInject<IGameOverService> gameOverService
        )
        {
            _roundStateFactory = roundStateFactory;
            _staticDataService = staticDataService;
            _gameStateMachine = gameStateMachine;
            _gameOverService = gameOverService;
        }

        public void CreateRoundStateController()
        {
            var staticData = _staticDataService.GetStaticData<RoundStateStaticData>();
            _daysData = staticData.Days;

            DayData dayData = GetDayData(_currentRound);

            _roundStateFactory.CreateRoundStateController()
                .AddRound(_currentRound)
                .AddDayCost(dayData.PlayCost)
                .AddDay(_currentDay)
                ;
        }

        public void RoundComplete()
        {
            _currentRound++;
            _gameStateMachine.Enter<RoundCompletionLoopState>();
        }

        public void PrepareToNextRound()
        {
            _gameStateMachine.Enter<RoundPreparationLoopState>();
        }

        public void DayComplete()
        {
            _currentDay++;
        }

        public void TryLoadNextLevel()
        {
            LoadNextLevelAsync().Forget();
        }

        public void GameOver()
        {
            _currentRound = 0;
            _currentDay = 0;
            _gameOverService.Value.GameOver();
        }

        private async UniTask LoadNextLevelAsync()
        {
            await DelaySeconds(1, new CancellationToken());

            DayData dayData = GetDayData(_currentRound);

            var loadLevelPayloadParameters = new LoadLevelPayloadParameters(dayData.SceneId.ToString());
            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(loadLevelPayloadParameters);
        }

        private DayData GetDayData(int currentRound)
        {
            foreach (DayData data in _daysData)
            {
                if (data.RoundId >= currentRound)
                    return data;
            }

            return _daysData[^1];
        }
    }
}