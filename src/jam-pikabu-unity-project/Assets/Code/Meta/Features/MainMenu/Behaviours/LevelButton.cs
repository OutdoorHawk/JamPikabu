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
        }

        public void SetSelectedView()
        {
            SelectedBg.EnableElement();
        }

        public void SetDeselectedView()
        {
            SelectedBg.DisableElement();
        }

        private void SelectLevel()
        {
            if (_mapMenuService.SelectedDayId == DayId)
                _mapMenuService.SetDayDeselected(DayId);
            else
                _mapMenuService.SetDaySelected(DayId);
        }
    }
}