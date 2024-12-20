using System;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;

namespace Code.Meta.Features.MainMenu.Service
{
    public class MapMenuService : IMapMenuService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;
        public event Action OnSelectionChanged;
        public int SelectedDayId { get; private set; }
        public bool DayIsSelected { get; private set; }

        private DaysStaticData DaysStaticData => _staticDataService.GetStaticData<DaysStaticData>();
        private MapBlocksStaticData BlocksStaticData => _staticDataService.GetStaticData<MapBlocksStaticData>();

        public MapMenuService(IStaticDataService staticDataService, IDaysService daysService)
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
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

        public MapBlockData GetMapBlockData(int currentDay)
        {
            return BlocksStaticData.GetMapBlockDataByDayId(currentDay);
        }

        public bool MapBlockIsAvailable(int dayId)
        {
            MapBlockData mapBlockData = BlocksStaticData.GetMapBlockDataByDayId(dayId);
            return MapBlockIsAvailable(mapBlockData);
        }

        public bool MapBlockIsAvailable(MapBlockData mapBlockData)
        {
            int starsEarned = _daysService.GetAllEarnedStars();
            
            if (mapBlockData == null)
                return false;

            if (starsEarned >= mapBlockData.StarsNeedToUnlock)
                return true;

            return false;
        }
    }
}