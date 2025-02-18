using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.MainMenu.Service;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class LevelButton : MonoBehaviour
    {
        public Button Button;
        public Animator Animator;
        public Image SelectedBg;
        public UIShiny SelectedShiny;
        public TMP_Text LevelNumber;
        public Transform StarsParent;
        public Image[] Stars;
        public Animator[] StarAnimators;
        public GameObject BossIcon;
        public int DayId;
        [ReadOnly] public bool Inactive;
        [ReadOnly] public bool Locked;

        private Image[] _starIcons;
        private IMapMenuService _mapMenuService;
        private IDaysService _daysService;
        private IStaticDataService _staticDataService;
        private ISoundService _soundService;

        [Inject]
        private void Construct
        (
            IMapMenuService mapMenuService,
            IDaysService daysService,
            IStaticDataService staticDataService,
            ISoundService soundService
        )
        {
            _soundService = soundService;
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
            InitStarsAnimation();
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
            Animator.SetBool(AnimationParameter.Selected.AsHash(), true);
            SelectedShiny.enabled = true;
        }

        public void SetDeselectedView()
        {
            SelectedBg.DisableElement();
            SelectedShiny.enabled = false;
            Animator.SetBool(AnimationParameter.Selected.AsHash(), false);
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

        private async UniTaskVoid InitStarsAnimation()
        {
            if (_daysService.TryGetDayProgress(DayId, out DayProgressData progress) == false)
                return;
            
            if (progress.StarsEarnedSeen == progress.StarsEarned)
                return;
        
            _daysService.SyncStarsSeen(DayId);
            
            for (int i = progress.StarsEarnedSeen; i < progress.StarsEarned; i++) 
                Stars[i].DisableElement();
            
            await DelaySeconds(0.75f, destroyCancellationToken);

            for (int i = progress.StarsEarnedSeen; i < progress.StarsEarned; i++)
            {
                Stars[i].EnableElement();
                StarAnimators[i].SetTrigger(AnimationParameter.Open.AsHash());
                _soundService.PlayOneShotSound(SoundTypeId.StarReceive);
                await DelaySeconds(0.25f, destroyCancellationToken);
            }
        }

        private void InitBoss()
        {
            bool isBossDay = _daysService.GetDayData(DayId).IsBossDay;
            BossIcon.gameObject.SetActive(isBossDay);
        }
    }
}