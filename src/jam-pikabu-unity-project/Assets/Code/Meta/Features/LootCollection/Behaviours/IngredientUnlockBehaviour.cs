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
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.Features.MainMenu.Windows;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static System.Threading.CancellationTokenSource;

namespace Code.Meta.Features.LootCollection.Behaviours
{
    public class IngredientUnlockBehaviour : MonoBehaviour
    {
        public Button UnlockButton;
        public Button FreeUpgradeButton;
        public Image[] IngredientIcons;
        public RectTransform FlyIconRect;
        public TMP_Text ReadyToUnlockText;
        public TMP_Text LockedText;
        public Animator UnlockIngredientAnimator;
        public UniversalTimer UpgradeTimer;

        private LootTypeId _unlocksIngredient;

        private IStaticDataService _staticData;
        private IWindowService _windowService;
        private ILootCollectionService _lootCollectionService;
        private CancellationTokenSource _fillToken;
        private ICurrencyFactory _currencyFactory;

        private LootProgressionStaticData LootData => _staticData.GetStaticData<LootProgressionStaticData>();

        [Inject]
        private void Construct
        (
            IStaticDataService staticData,
            IWindowService windowService,
            ILootCollectionService lootCollectionService,
            ICurrencyFactory currencyFactory
        )
        {
            _currencyFactory = currencyFactory;
            _lootCollectionService = lootCollectionService;
            _windowService = windowService;
            _staticData = staticData;
        }

        private void Awake()
        {
            gameObject.DisableElement();
        }

        private void Start()
        {
            UnlockButton.onClick.AddListener(ProceedUnlockClicked);
            FreeUpgradeButton.onClick.AddListener(FreeUpgradeClicked);
        }

        private void OnDestroy()
        {
            UnlockButton.onClick.RemoveAllListeners();
            FreeUpgradeButton.onClick.RemoveAllListeners();
            _fillToken?.Cancel();
        }

        public void InitLocked(LootTypeId unlocksIngredient)
        {
            ResetAll();
            Init(unlocksIngredient);
            UnlockButton.EnableElement();
            UnlockButton.interactable = false;
            LockedText.EnableElement();
        }

        public void InitReadyToCollect(LootTypeId unlocksIngredient)
        {
            ResetAll();
            Init(unlocksIngredient);
            UnlockButton.EnableElement();
            UnlockButton.interactable = true;
            ReadyToUnlockText.EnableElement();
            UnlockIngredientAnimator.SetBehaviorEnabled();
        }

        public void InitAwaitUpgrade(LootTypeId unlocksIngredient)
        {
            ResetAll();

            Init(unlocksIngredient);

            if (_lootCollectionService.CanUpgradeForFree(unlocksIngredient) == false)
                InitWaitUpgradeIdle();
            else
                InitReadyToFreeUpgrade();
        }

        private void ResetAll()
        {
            FreeUpgradeButton.DisableElement();
            UnlockButton.DisableElement();
            UnlockIngredientAnimator.SetBehaviorDisabled();
            LockedText.DisableElement();
            ReadyToUnlockText.DisableElement();
        }

        private void Init(LootTypeId unlocksIngredient)
        {
            gameObject.EnableElement();
            _unlocksIngredient = unlocksIngredient;
            var lootSettings = _staticData.GetStaticData<LootSettingsStaticData>();
            foreach (var icon in IngredientIcons)
                icon.sprite = lootSettings.GetConfig(_unlocksIngredient).Icon;
        }

        private void InitReadyToFreeUpgrade()
        {
            UnlockIngredientAnimator.SetTrigger(AnimationParameter.Upgrade.AsHash());
            UpgradeTimer.DisableElement();
            UpgradeTimer.StopTimer();
            FreeUpgradeButton.EnableElement();
        }

        private void InitWaitUpgradeIdle()
        {
            UnlockIngredientAnimator.SetBehaviorEnabled();
            UpgradeTimer.StartTimer(GetTimeFunc, () => InitAwaitUpgrade(_unlocksIngredient));
            UpdateFillAmountAsync().Forget();
        }

        private async UniTaskVoid UpdateFillAmountAsync()
        {
            _fillToken?.Cancel();
            _fillToken = CreateLinkedTokenSource(destroyCancellationToken);

            Image fillImage = IngredientIcons[0];
            LootProgressionData data = LootData.GetConfig(_unlocksIngredient);

            float maxWaitTimeSeconds = data.FreeUpgradeTimeHours * 60 * 60;
            int timeLeftSeconds = GetTimeFunc();

            while (timeLeftSeconds > 0)
            {
                timeLeftSeconds = GetTimeFunc();
                float factor = timeLeftSeconds / maxWaitTimeSeconds;
                fillImage.fillAmount = factor;
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _fillToken.Token);
            }

            fillImage.fillAmount = 1;
        }

        private void ProceedUnlockClicked()
        {
            if (_unlocksIngredient is LootTypeId.Unknown)
                return;

            UnlockButton.interactable = false;
            UnlockIngredient();
            CollectNewIngredient().Forget();
        }

        private void FreeUpgradeClicked()
        {
            UnlockIngredientAnimator.SetTrigger(AnimationParameter.Idle.AsHash());
            MoveIngredientToShop(from: IngredientIcons[0].transform.position);
        }

        private void UnlockIngredient()
        {
            CreateMetaEntity.Empty()
                .With(x => x.isUnlockLootRequest = true)
                .AddLootTypeId(_unlocksIngredient);
        }

        private async UniTaskVoid CollectNewIngredient()
        {
            await UnlockIngredientAnimator.WaitForAnimationCompleteAsync(AnimationParameter.Collect.AsHash(), destroyCancellationToken);
            MoveIngredientToShop(from: FlyIconRect.transform.position);
            await UniTask.Yield(destroyCancellationToken);
            InitAwaitUpgrade(_unlocksIngredient);
        }

        private void MoveIngredientToShop(Vector3 from)
        {
            if (_windowService.TryGetWindow(out MainMenuWindow mainMenuWindow) == false)
                return;

            Transform shopButton = mainMenuWindow.ShopButton.transform;

            var parameters = new CurrencyAnimationParameters()
            {
                Count = 1,
                Sprite = IngredientIcons[1].sprite,
                EndPosition = shopButton.position,
                StartPosition = from,
                LinkObject = gameObject
            };

            _currencyFactory.PlayCurrencyAnimation(parameters);
        }

        private int GetTimeFunc()
        {
            return _lootCollectionService.GetTimeLeftToFreeUpgrade(_unlocksIngredient);
        }
    }
}