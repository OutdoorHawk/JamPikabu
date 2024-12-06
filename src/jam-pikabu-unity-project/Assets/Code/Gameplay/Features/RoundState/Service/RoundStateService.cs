using System.Collections.Generic;
using System.Threading;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.StaticData;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.RoundState.Service
{
    public class RoundStateService : IRoundStateService
    {
        private readonly IRoundStateFactory _roundStateFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly IGameStateMachine _gameStateMachine;

        private List<DayData> _daysData;
        private int _currentDay;

        public int CurrentDay => _currentDay;

        public RoundStateService
        (
            IRoundStateFactory roundStateFactory,
            IStaticDataService staticDataService,
            IGameStateMachine gameStateMachine
        )
        {
            _roundStateFactory = roundStateFactory;
            _staticDataService = staticDataService;
            _gameStateMachine = gameStateMachine;
        }

        public void BeginDay()
        {
            var staticData = _staticDataService.GetStaticData<RoundStateStaticData>();
            _daysData = staticData.Days;
            _currentDay++;
            
            DayData dayData = GetDayData(_currentDay);

            _roundStateFactory.CreateRoundStateController()
                .AddDayCost(dayData.PlayCost)
                .AddDay(_currentDay)
                ;
        }

        public void RoundEnd()
        {
            _gameStateMachine.Enter<RoundCompletionLoopState>();
        }

        public void PrepareToNextRound()
        {
            _gameStateMachine.Enter<RoundPreparationLoopState>();
        }

        public void DayComplete()
        {
            LoadNextLevelAsync().Forget();
        }

        public void GameOver()
        {
            _currentDay = 0;
        }

        private async UniTask LoadNextLevelAsync()
        {
            _gameStateMachine.Enter<GameOverState>();
            
            await DelaySeconds(1, new CancellationToken());

            DayData dayData = GetDayData(_currentDay + 1);

            var loadLevelPayloadParameters = new LoadLevelPayloadParameters(dayData.SceneId.ToString());
            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(loadLevelPayloadParameters);
        }

        private DayData GetDayData(int currentRound)
        {
            foreach (DayData data in _daysData)
            {
                if (data.Day >= currentRound)
                    return data;
            }

            return _daysData[^1];
        }
    }
}