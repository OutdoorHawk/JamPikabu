using System;
using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Configs;

namespace Code.Meta.Features.Days.Service
{
    public class DaysService : IDaysService
    {
        public event Action OnEnterRoundPreparation;
        public event Action OnDayBegin;
        public event Action OnDayComplete;

        private readonly IRoundStateFactory _roundStateFactory;
        private readonly IStaticDataService _staticDataService;

        private List<DayData> _daysData;
        private DayData _currentDayData;

        private int _currentDay = 1;

        private readonly List<DayProgressData> _daysProgress = new();
        private readonly Dictionary<int, DayProgressData> _daysProgressByDayId = new();

        public int CurrentDay => _currentDay;
        public int MaxDays => _staticDataService.GetStaticData<DaysStaticData>().Configs.Count;
        private DaysStaticData DaysStaticData => _staticDataService.GetStaticData<DaysStaticData>();

        public DaysService
        (
            IRoundStateFactory roundStateFactory,
            IStaticDataService staticDataService
        )
        {
            _roundStateFactory = roundStateFactory;
            _staticDataService = staticDataService;
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

        public int GetStarsEarnedForDay(int day)
        {
            return _daysProgressByDayId.TryGetValue(day, out DayProgressData dayProgressData) 
                ? dayProgressData.StarsEarned 
                : 0;
        }

        public void BeginDay()
        {
            var staticData = _staticDataService.GetStaticData<DaysStaticData>();
            _daysData = staticData.Configs;

            _currentDayData = GetDayData(_currentDay);

            _roundStateFactory.CreateRoundStateController()
                .AddRoundDuration(_currentDayData.RoundDuration)
                ;

            OnDayBegin?.Invoke();
        }

        public void RoundEnd()
        {
        }

        public void EnterRoundPreparation()
        {
            OnEnterRoundPreparation?.Invoke();
        }

        public void DayComplete()
        {
            OnDayComplete?.Invoke();
        }

        public bool CheckAllDaysComplete()
        {
            return _currentDay >= MaxDays;
        }

        public DayData GetDayData()
        {
            return _currentDayData;
        }

        public DayData GetDayData(int currentDay)
        {
            return GetDayDataInternal(currentDay);
        }

        private DayData GetDayDataInternal(int currentDay)
        {
            return DaysStaticData.GetDayData(currentDay);
        }
    }
}