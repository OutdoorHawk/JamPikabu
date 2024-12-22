using System;
using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Common.Time.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Localization;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.Features.MainMenu.Service;
using Code.Meta.Features.MainMenu.Windows;
using Code.Meta.UI.Common.Replenish;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static System.Threading.CancellationTokenSource;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Meta.Features.MapBlocks.Behaviours
{
    public class UnlockableIngredient : MonoBehaviour
    {
        public Button UnlockButton;
        public Button FreeUpgradeButton;
        public Image[] IngredientIcons;
        public RectTransform FlyToShopStartPosition;
        public TMP_Text ReadyToUnlockText;
        public Animator UnlockIngredientAnimator;
        public UniversalTimer UpgradeTimer;
        public float UnlockMoveDuration = 0.6f;

        public LootTypeId UnlocksIngredient { get; private set; }

        public bool ReadyToUnlock { get; private set; }

        private IStaticDataService _staticData;
        private IWindowService _windowService;
        private ILootCollectionService _lootCollectionService;
        private CancellationTokenSource _fillToken;
        private ICurrencyFactory _currencyFactory;
        private IDaysService _daysService;
        private IMapMenuService _mapMenuService;
        private MapBlockData _mapBlockData;
        private IUIFactory _uiFactory;
        private ISoundService _soundService;
        private ILocalizationService _localizationService;

        private Image FillIcon => IngredientIcons[0];
        private Image BigFlyIcon => IngredientIcons[1];
        private Image GrayIcon => IngredientIcons[2];

        private LootProgressionStaticData LootData => _staticData.GetStaticData<LootProgressionStaticData>();
        private MapBlocksStaticData MapBlocksData => _staticData.GetStaticData<MapBlocksStaticData>();

        [Inject]
        private void Construct
        (
            IStaticDataService staticData,
            IWindowService windowService,
            ILootCollectionService lootCollectionService,
            ICurrencyFactory currencyFactory,
            IDaysService daysService,
            IMapMenuService mapMenuService,
            IUIFactory uiFactory,
            ISoundService soundService,
            ILocalizationService localizationService
        )
        {
            _localizationService = localizationService;
            _soundService = soundService;
            _uiFactory = uiFactory;
            _mapMenuService = mapMenuService;
            _daysService = daysService;
            _currencyFactory = currencyFactory;
            _lootCollectionService = lootCollectionService;
            _windowService = windowService;
            _staticData = staticData;
        }

        private void Start()
        {
            _lootCollectionService.OnFreeUpgradeTimeEnd += InitializeState;
            _lootCollectionService.OnUpgraded += InitializeState;
            UnlockButton.onClick.AddListener(ProceedUnlockClicked);
            FreeUpgradeButton.onClick.AddListener(FreeUpgradeClicked);
        }

        private void OnDestroy()
        {
            _lootCollectionService.OnFreeUpgradeTimeEnd -= InitializeState;
            _lootCollectionService.OnUpgraded -= InitializeState;
            UnlockButton.onClick.RemoveAllListeners();
            FreeUpgradeButton.onClick.RemoveAllListeners();
            _fillToken?.Cancel();
        }

        public void Initialize(MapBlockData mapBlockData)
        {
            _mapBlockData = mapBlockData;
            InitializeState();
        }

        private void InitializeState()
        {
            gameObject.DisableElement();
            TryInitState();
        }

        private void TryInitState()
        {
            MapBlockData mapBlockData = _mapBlockData;
            LootTypeId unlocksIngredient = mapBlockData.UnlocksIngredient;

            if (mapBlockData.UnlocksIngredient == LootTypeId.None)
                return;

            if (CheckPreviousDayIsCompleted() == false)
            {
                InitLocked(unlocksIngredient);
                return;
            }

            if (CheckIngredientAlreadyUnlocked(unlocksIngredient))
            {
                InitFreeUpgradeState(unlocksIngredient);
                return;
            }

            InitReadyToUnlock(unlocksIngredient);
        }

        private bool CheckIngredientAlreadyUnlocked(LootTypeId unlocksIngredient)
        {
            return _lootCollectionService.LootLevels.TryGetValue(unlocksIngredient, out _);
        }

        private bool CheckPreviousDayIsCompleted()
        {
            int lowestDay = _mapBlockData.DaysRange.x;
            int previousDayCompleted = lowestDay - 1;

            if (_daysService.TryGetDayProgress(previousDayCompleted, out _) == false)
                return false;

            return true;
        }

        private void InitLocked(LootTypeId unlocksIngredient)
        {
            ResetAll();
            Init(unlocksIngredient);
            UnlockIngredientAnimator.SetTrigger(AnimationParameter.Locked.AsHash());
            UnlockButton.EnableElement();
            UnlockButton.interactable = false;
            BigFlyIcon.EnableElement();
        }

        private void InitReadyToUnlock(LootTypeId unlocksIngredient)
        {
            ResetAll();
            Init(unlocksIngredient);
            UnlockButton.EnableElement();
            UnlockButton.interactable = true;
            ReadyToUnlockText.EnableElement();
            UnlockIngredientAnimator.SetTrigger(AnimationParameter.Ready.AsHash());
            ReadyToUnlock = true;
            BigFlyIcon.EnableElement();
        }

        private void InitFreeUpgradeState(LootTypeId type)
        {
            ResetAll();
            
            if (_lootCollectionService.CanUpgradeForFree(type) == false)
                return;

            Init(type);
            
            if (_mapMenuService.MapBlockIsAvailable(_mapBlockData) == false)
                return;

            if (_lootCollectionService.UpgradedForMaxLevel(type))
            {
                InitMaxLevelReached();
                return;
            }

            if (_lootCollectionService.TimeToFreeUpgradePassed(type) == false)
                InitWaitUpgradeIdle();
            else
                InitReadyToFreeUpgrade();
        }

        private void ResetAll()
        {
            _uiFactory.SetRaycastAvailable(true);
            BigFlyIcon.DisableElement();
            FillIcon.DisableElement();
            GrayIcon.DisableElement();
            ReadyToUnlock = false;
            gameObject.DisableElement();
            FreeUpgradeButton.DisableElement();
            UnlockButton.DisableElement();
            ReadyToUnlockText.DisableElement();
            UpgradeTimer.TimerText.text = "00:00";
            UpgradeTimer.StopTimer();
            _fillToken?.Cancel();
            FreeUpgradeButton.interactable = false;
        }

        private void Init(LootTypeId unlocksIngredient)
        {
            gameObject.EnableElement();
            UnlocksIngredient = unlocksIngredient;
            var lootSettings = _staticData.GetStaticData<LootSettingsStaticData>();
            foreach (var icon in IngredientIcons)
                icon.sprite = lootSettings.GetConfig(UnlocksIngredient).Icon;
        }

        private void InitMaxLevelReached()
        {
            GrayIcon.EnableElement();
            UpgradeTimer.TimerText.text = "Max";
            FillIcon.fillAmount = 1;
        }

        private void InitReadyToFreeUpgrade()
        {
            GrayIcon.EnableElement();
            FillIcon.EnableElement();
            FreeUpgradeButton.interactable = true;
            UnlockIngredientAnimator.SetTrigger(AnimationParameter.Upgrade.AsHash());
            UpgradeTimer.StopTimer();
            FreeUpgradeButton.EnableElement();
            FillIcon.fillAmount = 1;
        }

        private void InitWaitUpgradeIdle()
        {
            GrayIcon.EnableElement();
            FillIcon.EnableElement();
            UpgradeTimer.StartTimer(GetTimeFunc);
            UpdateFillAmountAsync().Forget();
        }

        private async UniTaskVoid UpdateFillAmountAsync()
        {
            _fillToken?.Cancel();
            _fillToken = CreateLinkedTokenSource(destroyCancellationToken);

            Image fillImage = FillIcon;
            MapBlockData mapBlockData = MapBlocksData.GetMapBlockDataByLinkedIngredient(UnlocksIngredient);

            float maxWaitTimeSeconds = mapBlockData.FreeUpgradeTimeSeconds;
            int timeLeftSeconds = GetTimeFunc();

            while (timeLeftSeconds > 0)
            {
                timeLeftSeconds = GetTimeFunc();
                float factor = 1 - (timeLeftSeconds / maxWaitTimeSeconds);
                fillImage.fillAmount = factor;
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _fillToken.Token);
            }

            fillImage.fillAmount = 1;
        }

        private void ProceedUnlockClicked()
        {
            if (UnlocksIngredient is LootTypeId.None)
                return;

            UnlockButton.interactable = false;
            CollectNewIngredient().Forget();
        }

        private void FreeUpgradeClicked()
        {
            FreeUpgradeButton.interactable = false;
            UnlockIngredientAnimator.SetTrigger(AnimationParameter.Idle.AsHash());
            MoveIngredientToShop(from: FillIcon.transform.position);
            _soundService.PlayOneShotSound(SoundTypeId.CollectIngredient);

            CreateMetaEntity.Empty()
                .With(x => x.isUpgradeLootRequest = true)
                .With(x => x.isFreeUpgradeRequest = true)
                .AddLootTypeId(UnlocksIngredient)
                .AddGold(0);
        }

        private async UniTaskVoid CollectNewIngredient()
        {
            CancellationToken cancellationToken = destroyCancellationToken;
            _uiFactory.SetRaycastAvailable(false);
            MoveIngredientToShop(from: FlyToShopStartPosition.transform.position, firstUnlock: true);
            _soundService.PlayOneShotSound(SoundTypeId.CollectIngredient);

            if (_lootCollectionService.CanUpgradeForFree(UnlocksIngredient) == false)
            {
                gameObject.DisableElement();
                return;
            }

            UnlockIngredientAnimator.WaitForAnimationCompleteAsync(AnimationParameter.Collect.AsHash(), cancellationToken).Forget();

            await DelaySeconds(0.5f, cancellationToken);

            BigFlyIcon.rectTransform
                .DOScale(1, UnlockMoveDuration)
                .SetLink(gameObject);
            
            await BigFlyIcon.rectTransform
                    .DOMove(FillIcon.transform.position, UnlockMoveDuration)
                    .SetLink(gameObject)
                    .AsyncWaitForCompletion()
                ;
            
            _soundService.PlayOneShotSound(SoundTypeId.Construction_Place);

            GrayIcon.EnableElement();
            BigFlyIcon.rectTransform.DisableElement();
            FillIcon.EnableElement();
            FillIcon.fillAmount = 1;

            await FillIcon
                    .DOFillAmount(0, 0.75f)
                    .SetLink(gameObject)
                    .AsyncWaitForCompletion()
                ;

            await UniTask.Yield(cancellationToken);
            UnlockIngredient();
            _uiFactory.SetRaycastAvailable(true);
        }

        private void UnlockIngredient()
        {
            CreateMetaEntity.Empty()
                .With(x => x.isUnlockLootRequest = true)
                .AddLootTypeId(UnlocksIngredient);
        }

        private void MoveIngredientToShop(Vector3 from, bool firstUnlock = false)
        {
            if (_windowService.TryGetWindow(out MainMenuWindow mainMenuWindow) == false)
                return;

            Transform shopButton = mainMenuWindow.ShopButton.transform;

            string prefix;

            if (firstUnlock == false)
                prefix = $"+1 {_localizationService["MAIN MENU/LVL"]}";
            else
            {
                prefix = string.Empty;
            }

            var parameters = new CurrencyAnimationParameters()
            {
                Count = 1,
                OverrideText = true,
                TextPrefix = prefix,
                AnimationName = "UnlockLoot",
                Sprite = BigFlyIcon.sprite,
                EndPosition = shopButton.position,
                StartPosition = from,
                LinkObject = gameObject,
                StartReplenishCallback = () => shopButton.GetComponent<IReplenishAnimator>().Replenish()
            };

            _currencyFactory.PlayCurrencyAnimation(parameters);
        }

        private int GetTimeFunc()
        {
            return _lootCollectionService.GetTimeLeftToFreeUpgrade(UnlocksIngredient);
        }
    }
}