using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Days.Service;
using UnityEngine;

namespace Code.Meta.Features.BonusLevel.Service
{
    public class BonusLevelService : IBonusLevelService
    {
        private readonly ITimeService _timeService;
        private readonly IStaticDataService _staticData;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IDaysService _daysService;

        private int _nextBonusLevelAvailableTimeStamp;

        private BonusLevelStaticData BonusLevelStaticData => _staticData.GetStaticData<BonusLevelStaticData>();

        public BonusLevelService
        (
            ITimeService timeService,
            IStaticDataService staticData,
            IGameStateMachine gameStateMachine,
            IDaysService daysService
        )
        {
            _timeService = timeService;
            _staticData = staticData;
            _gameStateMachine = gameStateMachine;
            _daysService = daysService;
        }

        public void UpdateTimeToNextBonusLevel(int bonusLevelAvailableTimeStamp)
        {
            _nextBonusLevelAvailableTimeStamp = bonusLevelAvailableTimeStamp;
        }

        public void LoadBonusLevel()
        {
            BonusLevelData bonusLevelData = BonusLevelStaticData.Configs[0];
            int time = _timeService.TimeStamp + bonusLevelData.ResetTimeSeconds;
            UpdateTimeToNextBonusLevel(time);

            CreateMetaEntity
                .Empty()
                .With(x => x.isBonusLevelAvailableTimer = true)
                .AddBonusLevelAvailableTime(time);

            _daysService.SetBonusLevel(bonusLevelData);

            List<SceneTypeId> scenes = bonusLevelData.SceneTypeId;
            
            var parameters = new LoadLevelPayloadParameters()
            {
                LevelName = scenes[Random.Range(0, scenes.Count)].ToString()
            };
            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(parameters);
        }

        public bool CanPlayBonusLevel()
        {
            if (_nextBonusLevelAvailableTimeStamp == 0)
                return true;

            if (GetTimeToBonusLevel() > 0)
                return false;

            return true;
        }

        public int GetTimeToBonusLevel()
        {
            int diff = _nextBonusLevelAvailableTimeStamp - _timeService.TimeStamp;
            return diff.ZeroIfNegative();
        }
    }
}