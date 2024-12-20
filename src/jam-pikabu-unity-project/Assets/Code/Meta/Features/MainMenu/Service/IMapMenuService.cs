using System;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.DayLootSettings.Configs;

namespace Code.Meta.Features.MainMenu.Service
{
    public interface IMapMenuService
    {
        event Action OnSelectionChanged;
        int SelectedDayId { get; }
        bool DayIsSelected { get; }
        void SetDaySelected(int dayId);
        void SetDayDeselected(int dayId);
        SceneTypeId GetSelectedScene();
        MapBlockData GetMapBlockData(int currentDay);
        bool MapBlockIsAvailable(int dayId);
        bool MapBlockIsAvailable(MapBlockData mapBlockData);
    }
}