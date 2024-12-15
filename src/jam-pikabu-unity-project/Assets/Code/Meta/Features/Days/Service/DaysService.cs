using System;
using System.Collections.Generic;
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

        public int CurrentDay => _currentDay;
        public int MaxDays => _staticDataService.GetStaticData<DaysStaticData>().Configs.Count;

        public DaysService
        (
            IRoundStateFactory roundStateFactory,
            IStaticDataService staticDataService
        )
        {
            _roundStateFactory = roundStateFactory;
            _staticDataService = staticDataService;
        }

        public void SetCurrentDay(int day)
        {
            _currentDay = day;
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
            foreach (DayData data in _daysData)
            {
                if (data.Id >= currentDay)
                    return data;
            }

            return _daysData[^1];
        }
    }
}