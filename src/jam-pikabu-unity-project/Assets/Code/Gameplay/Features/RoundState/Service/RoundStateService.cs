using System;
using System.Collections.Generic;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.StaticData;

namespace Code.Gameplay.Features.RoundState.Service
{
    public class RoundStateService : IRoundStateService
    {
        public event Action OnEnterRoundPreparation;
        public event Action OnDayBegin;
        public event Action OnDayComplete;

        private readonly IRoundStateFactory _roundStateFactory;
        private readonly IStaticDataService _staticDataService;

        private List<DayData> _daysData;
        private DayData _currentDayData;

        private int _currentDay;

        public int CurrentDay => _currentDay;
        public int MaxDays => _staticDataService.GetStaticData<RoundStateStaticData>().Days.Count;

        public RoundStateService
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
            var staticData = _staticDataService.GetStaticData<RoundStateStaticData>();
            _daysData = staticData.Days;

            _currentDayData = GetDayData(_currentDay);

            _roundStateFactory.CreateRoundStateController()
                .AddDayCost(_currentDayData.PlayCost)
                .AddDay(_currentDay)
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
                if (data.Day >= currentDay)
                    return data;
            }

            return _daysData[^1];
        }
    }
}