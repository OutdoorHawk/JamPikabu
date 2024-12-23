using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Extensions;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Analytics;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Configs.Stars;
using UnityEngine;

namespace Code.Meta.Features.Days.Service
{
    public class DaysService : IDaysService
    {
        public event Action OnEnterRoundPreparation;
        public event Action OnDayBegin;
        public event Action OnDayComplete;

        private readonly IRoundStateFactory _roundStateFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly ISoundService _soundService;
        private readonly IAnalyticsService _analyticsService;

        private DayData _currentDayData;
        private BonusLevelData _bonusLevelData;
        
        private readonly List<DayProgressData> _daysProgress = new();
        private readonly Dictionary<int, DayProgressData> _daysProgressByDayId = new();
        public BonusLevelType BonusLevelType { get; private set; }

        public List<DayStarData> DayStarsData { get; } = new(3);

        public int CurrentDay => _currentDayData.Id;
        public int MaxDays => _staticDataService.GetStaticData<DaysStaticData>().Configs.Count;
        private DaysStaticData DaysStaticData => _staticDataService.GetStaticData<DaysStaticData>();
        private DayStarsStaticData DayStarsStaticData => _staticDataService.GetStaticData<DayStarsStaticData>();

        public DaysService
        (
            IRoundStateFactory roundStateFactory,
            IStaticDataService staticDataService,
            ISoundService soundService,
            IAnalyticsService analyticsService
        )
        {
            _roundStateFactory = roundStateFactory;
            _staticDataService = staticDataService;
            _soundService = soundService;
            _analyticsService = analyticsService;
        }

        public void SetBonusLevel(BonusLevelData type, SceneTypeId sceneTypeId)
        {
            _bonusLevelData = type;
            BonusLevelType = type.Type;

            DayProgressData dayProgressData = _daysProgress.Last();

            DayData dayData = DaysStaticData.GetDayData(dayProgressData.DayId);

            _currentDayData = new DayData()
            {
                DayGoldFactor = dayData.DayGoldFactor,
                SceneId = sceneTypeId,
                Id = dayData.Id,
            };
        }

        public void InitializeDays(IEnumerable<DayProgressData> daysProgress)
        {
            _daysProgressByDayId.Clear();
            _daysProgress.RefreshList(daysProgress);

            foreach (DayProgressData dayProgressData in _daysProgress)
                _daysProgressByDayId[dayProgressData.DayId] = dayProgressData;
        }

        public bool IsCompletedFirstLevel()
        {
            return _daysProgress.Count != 0;
        }

        public void SetActiveDay(DayData selectedDayId)
        {
            _currentDayData = selectedDayId;
        }

        public void SetActiveDay(int dayId)
        {
            _currentDayData = DaysStaticData.GetDayData(dayId);
        }

        public List<DayProgressData> GetDaysProgress()
        {
            return _daysProgress;
        }

        public bool TryGetDayProgress(int dayId, out DayProgressData dayProgress)
        {
            return _daysProgressByDayId.TryGetValue(dayId, out dayProgress);
        }

        public int GetStarsEarnedForDay(int day)
        {
            return _daysProgressByDayId.TryGetValue(day, out DayProgressData dayProgressData)
                ? dayProgressData.StarsEarned
                : 0;
        }

        public int GetAllEarnedStars()
        {
            return _daysProgress.Sum(data => data.StarsEarned);
        }

        public void BeginDay()
        {
            var staticData = _staticDataService.GetStaticData<DaysStaticData>();

            float roundDuration = GetRoundDuration();

            _roundStateFactory.CreateRoundStateController()
                .AddRoundDuration(roundDuration)
                ;

            if (_currentDayData.IsBossDay)
                _soundService.PlayMusic(SoundTypeId.SpecialGameplayMusic);
            if (BonusLevelType is BonusLevelType.GoldenCoins)
                _soundService.PlayMusic(SoundTypeId.SpecialGameplayMusic);

            InitStars();

            if (BonusLevelType is BonusLevelType.GoldenCoins)
            {
                _analyticsService.SendEvent(AnalyticsEventTypes.LevelStart, BonusLevelType.GoldenCoins.ToString());
            }
            else
            {
                _analyticsService.SendEvent(AnalyticsEventTypes.LevelStart, CurrentDay.ToString());
            }
           
            OnDayBegin?.Invoke();
        }

        public void RoundEnd()
        {
            
        }

        public void EnterRoundPreparation()
        {
            OnEnterRoundPreparation?.Invoke();
        }

        public void StarsRecieved(int starsReceived)
        {
            _analyticsService.SendEvent(AnalyticsEventTypes.LevelEnd, CurrentDay.ToString());
            _analyticsService.SendEvent(AnalyticsEventTypes.StarsEarned, starsReceived.ToString());
        }

        public void DayComplete()
        {
            if (BonusLevelType is BonusLevelType.GoldenCoins) 
                _analyticsService.SendEvent(AnalyticsEventTypes.LevelEnd, BonusLevelType.GoldenCoins.ToString());
            
            BonusLevelType = BonusLevelType.None;
            _bonusLevelData = null;
            _currentDayData = null;
            OnDayComplete?.Invoke();
        }

        public float GetRoundDuration()
        {
            if (BonusLevelType is BonusLevelType.GoldenCoins)
                return _bonusLevelData.RoundTimeOverride;

            float roundDuration = _currentDayData.IsBossDay
                ? DaysStaticData.BossRoundDuration
                : DaysStaticData.DefaultRoundDuration;

            return roundDuration;
        }

        public bool CheckDayUnlocked(int dayId)
        {
            int previousDay = dayId - 1;

            if (previousDay == 0)
                return true;

            if (_daysProgressByDayId.TryGetValue(previousDay, out DayProgressData _) == false)
                return false;

            return true;
        }

        public DayData GetDayData()
        {
            return _currentDayData;
        }

        public DayData GetDayData(int currentDay)
        {
            return GetDayDataInternal(currentDay);
        }

        public DayStarsSetup GetDayStarData()
        {
            return DayStarsStaticData.GetDayStarsData(_currentDayData.Id);
        }

        public DayStarsSetup GetDayStarData(int currentDay)
        {
            return DayStarsStaticData.GetDayStarsData(currentDay);
        }

        public float GetDayGoldFactor()
        {
            if (_currentDayData == null)
                return 1;

            if (BonusLevelType == BonusLevelType.GoldenCoins)
                return 1 * _currentDayData.DayGoldFactor * _bonusLevelData.GoldFactorModifier;

            return 1 * _currentDayData.DayGoldFactor;
        }

        private DayData GetDayDataInternal(int currentDay)
        {
            return DaysStaticData.GetDayData(currentDay);
        }

        private void InitStars()
        {
            DayStarsData.Clear();

            DayStarsSetup dayStarsSetup = DayStarsStaticData.GetDayStarsData(_currentDayData.Id);
            int ratingNeedAll = dayStarsSetup.RatingNeedAll;
            float[] starsFactorSetup = DayStarsStaticData.StarsFactorSetup;

            for (int i = 0; i < starsFactorSetup.Length; i++)
            {
                var dayStarData = new DayStarData
                {
                    RatingAmountNeed = Mathf.CeilToInt(ratingNeedAll * starsFactorSetup[i])
                };

                DayStarsData.Add(dayStarData);
            }
        }
    }
}