using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.MainMenu.Service;
using Coffee.UIExtensions;
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
        public UIShiny SelectedShiny;
        public TMP_Text LevelNumber;
        public Transform StarsParent;
        public Image[] Stars;
        public GameObject BossIcon;
        public int DayId;
        [ReadOnly] public bool Inactive;
        [ReadOnly] public bool Locked;

        private Image[] _starIcons;
        private IMapMenuService _mapMenuService;
        private IDaysService _daysService;
        private IStaticDataService _staticDataService;

        [Inject]
        private void Construct
        (
            IMapMenuService mapMenuService,
            IDaysService daysService,
            IStaticDataService staticDataService
        )
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
            _mapMenuService = mapMenuService;
        }

        private void Awake()
        {
            _starIcons = StarsParent.GetComponentsInChildren<Image>(true);
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
            SelectedShiny.enabled = true;
        }

        public void SetDeselectedView()
        {
            SelectedBg.DisableElement();
            SelectedShiny.enabled = false;
        }

        public void SetLevelLocked()
        {
            Button.interactable = false;
            Locked = true;
        }

        public void SelectLevel()
        {
            if (_mapMenuService.SelectedDayId == DayId)
                return;

            _mapMenuService.SetDaySelected(DayId);
        }

        private void Init()
        {
            InitLevel();
            InitStars();
            InitBoss();
        }

        private void InitLevel()
        {
            LevelNumber.text = DayId.ToString();
            gameObject.name = $"Day_{DayId}";
        }

        private void InitStars()
        {
            /*CurrencyConfig config = _staticDataService
                .GetStaticData<CurrencyStaticData>()
                .GetCurrencyConfig(CurrencyTypeId.Star);

            foreach (var star in _starIcons)
                star.sprite = config.Data.Icon;*/

            foreach (var star in Stars)
                star.DisableElement();

            if (_daysService.TryGetDayProgress(DayId, out var progress) == false)
                return;

            for (int i = 0; i < progress.StarsEarned; i++)
            {
                Stars[i].EnableElement();
            }
        }

        private void InitBoss()
        {
            bool isBossDay = _daysService.GetDayData(DayId).IsBossDay;
            BossIcon.gameObject.SetActive(isBossDay);
        }
    }
}