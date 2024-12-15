using System;
using Code.Infrastructure.SceneLoading;

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
    }
}