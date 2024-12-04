using System.Collections.Generic;
using System.Threading;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.StaticData;
using Code.Infrastructure.States.GameStates;
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

        private List<RoundData> _rounds;
        private int _currentRound = 1;

        public int CurrentRound => _currentRound;

        public RoundStateService(IRoundStateFactory roundStateFactory, 
            IStaticDataService staticDataService, 
            IGameStateMachine gameStateMachine,
            LazyInject<IGameOverService> gameOverService)
        {
            _roundStateFactory = roundStateFactory;
            _staticDataService = staticDataService;
            _gameStateMachine = gameStateMachine;
            _gameOverService = gameOverService;
        }

        public void CreateRoundStateController()
        {
            var staticData = _staticDataService.GetStaticData<RoundStateStaticData>();
            _rounds = staticData.Rounds;

            RoundData roundData = GetRoundData(_currentRound);

            _roundStateFactory.CreateRoundStateController()
                .AddRound(_currentRound)
                .AddRoundCost(roundData.PlayCost);
        }

        public void RoundComplete()
        {
            _currentRound++;
        }

        public void ResetCurrentRound()
        {
            _currentRound = 0;
        }

        public void TryLoadNextLevel()
        {
            LoadNextLevelAsync().Forget();
        }

        public void GameOver()
        {
            _gameOverService.Value.GameOver();
        }

        private async UniTask LoadNextLevelAsync()
        {
            _gameStateMachine.Enter<InventoryState>();

            await DelaySeconds(1, new CancellationToken());

            RoundData roundData = GetRoundData(_currentRound);

            var loadLevelPayloadParameters = new LoadLevelPayloadParameters(roundData.SceneId.ToString());
            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(loadLevelPayloadParameters);
        }

        private RoundData GetRoundData(int currentRound)
        {
            foreach (RoundData data in _rounds)
            {
                if (data.RoundId >= currentRound)
                    return data;
            }

            return _rounds[^1];
        }
    }
}