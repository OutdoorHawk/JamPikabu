using System;
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
        public event Action OnEnterRoundPreparation;
        public event Action OnDayBegin;
        public event Action OnDayComplete;
        
        private readonly IRoundStateFactory _roundStateFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly IGameStateMachine _gameStateMachine;

        private List<DayData> _daysData;
        private DayData _currentDayData;
        
        private int _currentDay;

        public int CurrentDay => _currentDay;
        public int MaxDays => _staticDataService.GetStaticData<RoundStateStaticData>().Days.Count;

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
            
            _currentDayData = GetDayData(_currentDay);

            _roundStateFactory.CreateRoundStateController()
                .AddDayCost(_currentDayData.PlayCost)
                .AddDay(_currentDay)
                ;
            
            OnDayBegin?.Invoke();
        }

        public void RoundEnd()
        {
            _gameStateMachine.Enter<RoundCompletionLoopState>();
        }

        public void PrepareToNextRound()
        {
            _gameStateMachine.Enter<RoundPreparationLoopState>();
            OnEnterRoundPreparation?.Invoke();
        }

        public void DayComplete()
        {
            LoadNextLevelAsync().Forget();
            OnDayComplete?.Invoke();
        }

        public DayData GetDayData()
        {
            return _currentDayData;
        }

        private DayData GetDayData(int currentDay)
        {
            foreach (DayData data in _daysData)
            {
                if (data.Day >= currentDay)
                    return data;
            }

            return _daysData[^1];
        }

        public void GameOver()
        {
            _currentDay = 0;
            _currentDayData = null;
        }

        private async UniTask LoadNextLevelAsync()
        {
            _gameStateMachine.Enter<GameOverState>();
            
            await DelaySeconds(1, new CancellationToken());

            DayData dayData = GetDayData(_currentDay + 1);

            var loadLevelPayloadParameters = new LoadLevelPayloadParameters(dayData.SceneId.ToString());
            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(loadLevelPayloadParameters);
        }
    }
}