using Code.Common.Extensions;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.MainMenu.Service;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class LevelButton : MonoBehaviour
    {
        public Button Button;
        public Image SelectedBg;
        public TMP_Text LevelNumber;
        public Image[] Stars;
        public int DayId;
        [ReadOnly] public bool Inactive;
        [ReadOnly] public bool Locked;

        private IMapMenuService _mapMenuService;
        private IDaysService _daysService;

        [Inject]
        private void Construct
        (
            IMapMenuService mapMenuService,
            IDaysService daysService
        )
        {
            _daysService = daysService;
            _mapMenuService = mapMenuService;
        }

        private void Awake()
        {
            if (_mapMenuService.SelectedDayId != DayId)
                SelectedBg.DisableElement();
        }

        private void Start()
        {
            Button.onClick.AddListener(SelectLevel);
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(SelectLevel);
        }

        public void InitLevel(DayData data)
        {
            DayId = data.Id;
            Init();
        }

        public void InitInactive()
        {
            Init();
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

        private void Init()
        {
            InitLevel();
            InitStars();
        }

        private void InitLevel()
        {
            LevelNumber.text = DayId.ToString();
            gameObject.name = $"Day_{DayId}";
        }

        private void InitStars()
        {
            foreach (var star in Stars)
                star.DisableElement();

            if (_daysService.TryGetDayProgress(DayId, out var progress) == false)
                return;

            for (int i = 0; i < progress.StarsEarned; i++)
            {
                Stars[i].EnableElement();
            }
        }
    }
}