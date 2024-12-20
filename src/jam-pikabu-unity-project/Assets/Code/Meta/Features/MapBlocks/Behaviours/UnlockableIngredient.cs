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
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Service;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.Features.MainMenu.Service;
using Code.Meta.Features.MainMenu.Windows;
using Code.Meta.UI.Common.Replenish;
using Cysharp.Threading.Tasks;
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
        public RectTransform FlyIconRect;
        public TMP_Text ReadyToUnlockText;
        public TMP_Text LockedText;
        public Animator UnlockIngredientAnimator;
        public UniversalTimer UpgradeTimer;

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
            IMapMenuService mapMenuService
        )
        {
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
            LockedText.EnableElement();
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
        }

        private void InitFreeUpgradeState(LootTypeId type)
        {
            ResetAll();

            if (_lootCollectionService.CanUpgradeForFree(type) == false)
                return;

            Init(type);

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
            ReadyToUnlock = false;
            gameObject.DisableElement();
            FreeUpgradeButton.DisableElement();
            UnlockButton.DisableElement();
            LockedText.DisableElement();
            ReadyToUnlockText.DisableElement();
            UpgradeTimer.DisableElement();
            UpgradeTimer.StopTimer();
            _fillToken?.Cancel();
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
            UpgradeTimer.EnableElement();
            UpgradeTimer.TimerText.text = "Max";
            IngredientIcons[0].fillAmount = 1;
        }

        private void InitReadyToFreeUpgrade()
        {
            UnlockIngredientAnimator.SetTrigger(AnimationParameter.Upgrade.AsHash());
            UpgradeTimer.DisableElement();
            UpgradeTimer.StopTimer();
            FreeUpgradeButton.EnableElement();
            IngredientIcons[0].fillAmount = 1;
        }

        private void InitWaitUpgradeIdle()
        {
            UpgradeTimer.EnableElement();
            UpgradeTimer.StartTimer(GetTimeFunc);
            UpdateFillAmountAsync().Forget();
        }

        private async UniTaskVoid UpdateFillAmountAsync()
        {
            _fillToken?.Cancel();
            _fillToken = CreateLinkedTokenSource(destroyCancellationToken);

            Image fillImage = IngredientIcons[0];
            MapBlockData mapBlockData = MapBlocksData.GetMapBlockDataByLinkedIngredient(UnlocksIngredient);

            float maxWaitTimeSeconds = mapBlockData.FreeUpgradeTimeHours * 60 * 60;
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
            UnlockIngredientAnimator.SetTrigger(AnimationParameter.Idle.AsHash());
            MoveIngredientToShop(from: IngredientIcons[0].transform.position);

            CreateMetaEntity.Empty()
                .With(x => x.isUpgradeLootRequest = true)
                .With(x => x.isFreeUpgradeRequest = true)
                .AddLootTypeId(UnlocksIngredient)
                .AddGold(0);
        }

        private async UniTaskVoid CollectNewIngredient()
        {
            MoveIngredientToShop(from: FlyIconRect.transform.position);
            await DelaySeconds(0.5f, destroyCancellationToken);
            
            if (_lootCollectionService.CanUpgradeForFree(UnlocksIngredient)) 
                await UnlockIngredientAnimator.WaitForAnimationCompleteAsync(AnimationParameter.Collect.AsHash(), destroyCancellationToken);
            else
                gameObject.DisableElement();
            
            await UniTask.Yield(destroyCancellationToken);
            UnlockIngredient();
        }

        private void UnlockIngredient()
        {
            CreateMetaEntity.Empty()
                .With(x => x.isUnlockLootRequest = true)
                .AddLootTypeId(UnlocksIngredient);
        }

        private void MoveIngredientToShop(Vector3 from)
        {
            if (_windowService.TryGetWindow(out MainMenuWindow mainMenuWindow) == false)
                return;

            Transform shopButton = mainMenuWindow.ShopButton.transform;

            var parameters = new CurrencyAnimationParameters()
            {
                Count = 1,
                AnimationName = "UnlockLoot",
                Sprite = IngredientIcons[1].sprite,
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