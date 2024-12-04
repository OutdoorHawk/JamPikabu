using System.Collections.Generic;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.StaticData;

namespace Code.Gameplay.Features.RoundState.Service
{
    public class RoundStateService : IRoundStateService
    {
        private readonly IRoundStateFactory _roundStateFactory;
        private readonly IStaticDataService _staticDataService;
        
        private List<RoundData> _rounds;
        private int _currentRound = 1;

        public RoundStateService(IRoundStateFactory roundStateFactory, IStaticDataService staticDataService)
        {
            _roundStateFactory = roundStateFactory;
            _staticDataService = staticDataService;
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