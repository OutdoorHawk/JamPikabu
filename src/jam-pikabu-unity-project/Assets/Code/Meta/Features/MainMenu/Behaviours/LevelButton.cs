using Code.Common.Extensions;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.MainMenu.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class LevelButton : MonoBehaviour
    {
        public Button Button;
        public Image SelectedBg;
        public int DayId;
        public bool Inactive;
        public bool Locked;

        private IMapMenuService _mapMenuService;

        [Inject]
        private void Construct(IMapMenuService mapMenuService)
        {
            _mapMenuService = mapMenuService;
        }

        private void Start()
        {
            Button.onClick.AddListener(SelectLevel);
            SelectedBg.DisableElement();
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(SelectLevel);
        }

        public void InitLevel(DayData data)
        {
            DayId = data.Id;
            gameObject.name = $"Day_{DayId}";
        }

        public void InitInactive()
        {
            Button.interactable = false;
            Inactive = true;
        }

        public void SetSelectedView()
        {
            SelectedBg.EnableElement();
        }

        public void SetDeselectedView()
        {
            SelectedBg.DisableElement();
        }

        public void SetLevelLocked()
        {
            Button.interactable = false;
            Locked = true;
        }

        public void SelectLevel()
        {
            if (_mapMenuService.SelectedDayId == DayId)
                _mapMenuService.SetDayDeselected(DayId);
            else
                _mapMenuService.SetDaySelected(DayId);
        }
    }
}