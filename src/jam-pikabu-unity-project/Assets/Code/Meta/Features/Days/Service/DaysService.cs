using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Extensions;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Analytics;
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

        private List<DayData> _daysData;
        private DayData _currentDayData;

        private int _currentDay = 1;

        private readonly List<DayProgressData> _daysProgress = new();
        private readonly Dictionary<int, DayProgressData> _daysProgressByDayId = new();

        public List<DayStarData> DayStarsData { get; } = new(3);

        public int CurrentDay => _currentDay;
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

        public void SetActiveDay(int selectedDayId)
        {
            _currentDay = selectedDayId;
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
            _daysData = staticData.Configs;

            _currentDayData = GetDayData(_currentDay);

            float roundDuration = GetRoundDuration();

            _roundStateFactory.CreateRoundStateController()
                .AddRoundDuration(roundDuration)
                ;

            if (_currentDayData.IsBossDay)
                _soundService.PlayMusic(SoundTypeId.SpecialGameplayMusic);

            InitStars();

            _analyticsService.SendEvent(AnalyticsEventTypes.LevelStart, _currentDay.ToString());
            OnDayBegin?.Invoke();
        }

        public void RoundEnd()
        {
        }

        public void EnterRoundPreparation()
        {
            OnEnterRoundPreparation?.Invoke();
        }

        public void DayComplete(int starsReceived)
        {
            _analyticsService.SendEvent(AnalyticsEventTypes.LevelEnd, _currentDay.ToString());
            _analyticsService.SendEvent(AnalyticsEventTypes.StarsEarned, starsReceived.ToString());
            OnDayComplete?.Invoke();
        }

        public float GetRoundDuration()
        {
            float roundDuration = _currentDayData.IsBossDay
                ? DaysStaticData.BossRoundDuration
                : DaysStaticData.DefaultRoundDuration;

            return roundDuration;
        }

        public bool CheckAllDaysComplete()
        {
            return _currentDay >= MaxDays;
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
                    RatingAmountNeed = Mathf.RoundToInt(ratingNeedAll * starsFactorSetup[i])
                };

                DayStarsData.Add(dayStarData);
            }
        }
    }
}