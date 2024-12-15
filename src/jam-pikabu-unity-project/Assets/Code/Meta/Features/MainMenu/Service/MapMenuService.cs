using System;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.Days.Configs;

namespace Code.Meta.Features.MainMenu.Service
{
    public class MapMenuService : IMapMenuService
    {
        private readonly IStaticDataService _staticDataService;
        public event Action OnSelectionChanged;
        public int SelectedDayId { get; private set; }
        public bool DayIsSelected { get; private set; }

        private DaysStaticData DaysStaticData => _staticDataService.GetStaticData<DaysStaticData>();

        public MapMenuService(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public void SetDaySelected(int dayId)
        {
            SelectedDayId = dayId;
            DayIsSelected = true;
            NotifyChanged();
        }

        public void SetDayDeselected(int dayId)
        {
            SelectedDayId = 0;
            DayIsSelected = false;
            NotifyChanged();
        }

        private void NotifyChanged()
        {
            OnSelectionChanged?.Invoke();
        }

        public SceneTypeId GetSelectedScene()
        {
            return DaysStaticData.GetDayData(SelectedDayId).SceneId;
        }
    }
}