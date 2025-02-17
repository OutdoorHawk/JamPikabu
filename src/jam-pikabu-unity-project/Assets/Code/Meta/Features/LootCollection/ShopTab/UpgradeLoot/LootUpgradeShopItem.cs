using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.Abilities.Config;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.UI.Common;
using Code.Meta.UI.PreviewItem;
using Code.Meta.UI.PreviewItem.Service;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.LootCollection.ShopTab.UpgradeLoot
{
    public class LootUpgradeShopItem : MonoBehaviour
    {
        public LootTypeId TypeId;
        public Image Icon;
        public Image RatingArrow;
        public TMP_Text Name;
        public Button UpgradeButton;
        public UIShiny UpgradeButtonShiny;
        public PriceInfo RatingFrom;
        public PriceInfo RatingTo;
        public PriceInfo UpgradePrice;
        public TMP_Text MaxReached;
        public Button IconButton;
        public GameObject InfoPin;

        public Image AvailableButton;
        public Image NotAvailableButton;
        public Color AvailableColor;
        public Color NotAvailableColor;

        private IStaticDataService _staticData;
        private IGameplayCurrencyService _gameplayCurrencyService;
        private ICurrencyFactory _currencyFactory;
        private ISoundService _soundService;
        private IPreviewWindowService _previewWindowService;

        private bool _firstInitComplete;
        private CostSetup _upgradePrice;
        private CurrencyTypeId _ratingCurrency;

        public LootTypeId Type => TypeId;
        public bool MaxLevelReached { get; private set; }
        private LootSettingsStaticData LootSettings => _staticData.Get<LootSettingsStaticData>();
        private LootProgressionStaticData ProgressionStaticData => _staticData.Get<LootProgressionStaticData>();

        [Inject]
        private void Construct
        (
            IStaticDataService staticData,
            IGameplayCurrencyService gameplayCurrencyService,
            ICurrencyFactory currencyFactory,
            ISoundService soundService,
            IPreviewWindowService previewWindowService
        )
        {
            _previewWindowService = previewWindowService;
            _soundService = soundService;
            _currencyFactory = currencyFactory;
            _gameplayCurrencyService = gameplayCurrencyService;
            _staticData = staticData;
        }

        private void Start()
        {
            _gameplayCurrencyService.CurrencyChanged += RefreshState;
            UpgradeButton.onClick.AddListener(PurchaseUpgrade);
            IconButton.onClick.AddListener(OpenPreviewWindow);
        }

        private void OnDestroy()
        {
            _gameplayCurrencyService.CurrencyChanged -= RefreshState;
            UpgradeButton.onClick.RemoveListener(PurchaseUpgrade);
            IconButton.onClick.RemoveListener(OpenPreviewWindow);
        }

        public void Init(in LootLevelsProgressionData item)
        {
            Icon.sprite = LootSettings.GetConfig(item.Type).Icon;
            TypeId = item.Type;
            _ratingCurrency = CurrencyTypeId.Plus;

            if (GetCurrentLevel(item) == null)
            {
                gameObject.DisableElement();
                return;
            }

            InitLocale(in item);
            InitCurrentLevel(in item);
            InitNextLevel(in item);
            InitUpgradePrice(in item);
            InitInfoButton(in item);
            UpdateAvailable();
            _firstInitComplete = true;
        }

        private void InitLocale(in LootLevelsProgressionData item)
        {
            LocalizedString localizedString = LootSettings.GetConfig(item.Type).LocalizedName;

            if (localizedString.IsEmpty == false)
                Name.text = localizedString.GetLocalizedString();
        }

        private void InitCurrentLevel(in LootLevelsProgressionData item)
        {
            LootSettingsData lootSetup = LootSettings.GetConfig(item.Type);
            LootLevelData currentLevel = GetCurrentLevel(item);
            RatingFrom.SetupPrice(lootSetup.BaseRatingValue + currentLevel.RatingBoostAmount, _ratingCurrency, withAnimation: _firstInitComplete);
        }

        private void InitNextLevel(in LootLevelsProgressionData item)
        {
            LootSettingsData lootSetup = LootSettings.GetConfig(item.Type);
            List<LootLevelData> levels = ProgressionStaticData.GetConfig(item.Type).Levels;

            if (item.Level + 1 >= levels.Count)
            {
                MaxLevelReached = true;
                return;
            }

            MaxLevelReached = false;
            LootLevelData nextLevel = levels[item.Level + 1];
            RatingTo.SetupPrice(lootSetup.BaseRatingValue + nextLevel.RatingBoostAmount, _ratingCurrency, withAnimation: _firstInitComplete);
        }

        private void InitUpgradePrice(in LootLevelsProgressionData item)
        {
            _upgradePrice = null;

            LootLevelData currentLevel = GetCurrentLevel(item);
            UpgradePrice.SetupPrice(currentLevel.Cost, withAnimation: _firstInitComplete);
            _upgradePrice = currentLevel.Cost;
        }

        private LootLevelData GetCurrentLevel(in LootLevelsProgressionData item)
        {
            LootProgressionData lootProgressionData = ProgressionStaticData.GetConfig(item.Type);

            if (lootProgressionData == null)
                return null;

            List<LootLevelData> levels = lootProgressionData.Levels;

            if (item.Level >= levels.Count)
                return levels[^1];

            return levels[item.Level];
        }

        private void InitInfoButton(in LootLevelsProgressionData item)
        {
            LootSettingsData lootSetup = LootSettings.GetConfig(item.Type);

            if (lootSetup.AbilityType == AbilityTypeId.None)
            {
                IconButton.enabled = false;
                InfoPin.DisableElement();
                return;
            }

            IconButton.enabled = true;
            InfoPin.EnableElement();
        }

        private void UpdateAvailable()
        {
            ResetAll();

            if (_upgradePrice == null)
                return;

            if (MaxLevelReached)
            {
                SetMaxLevelReached();
                return;
            }

            int goldAmount = _gameplayCurrencyService.GetCurrencyOfType(_upgradePrice.CurrencyType, false);

            if (goldAmount >= _upgradePrice.Amount)
                SetCanUpgrade();
            else
                SetNoMoneyForUpgrade();
        }

        private void ResetAll()
        {
            MaxReached.DisableElement();
            UpgradePrice.DisableElement();
            RatingTo.DisableElement();
            RatingArrow.DisableElement();
            UpgradeButton.enabled = false;
        }

        private void SetMaxLevelReached()
        {
            MaxReached.EnableElement();
            UpgradeButtonShiny.Stop();
            SetAvailableButtonColors();
        }

        private void SetNoMoneyForUpgrade()
        {
            UpgradePrice.EnableElement();
            RatingTo.EnableElement();
            RatingArrow.EnableElement();
            SetNotAvailableButtonColors();
        }

        private void SetCanUpgrade()
        {
            UpgradePrice.EnableElement();
            RatingTo.EnableElement();
            RatingArrow.EnableElement();
            UpgradeButton.enabled = true;
            UpgradeButtonShiny.Play();
            SetAvailableButtonColors();
        }

        private void RefreshState()
        {
            UpdateAvailable();
        }

        private void PurchaseUpgrade()
        {
            CreateMetaEntity
                .Empty()
                .With(x => x.isUpgradeLootRequest = true)
                .AddLootTypeId(Type)
                .AddGold(_upgradePrice.Amount)
                ;

            //PlayAnimation();
            RefreshState();
            _soundService.PlayOneShotSound(SoundTypeId.PurchasedQuota);
        }

        private void OpenPreviewWindow()
        {
            LootSettingsData lootSetup = LootSettings.GetConfig(Type);
            
            AbilityData abilityData = _staticData.Get<AbilityStaticData>().GetDataByType(lootSetup.AbilityType);
            LocalizedString text = abilityData.LocalizedDescription;

            var parameters = new PreviewItemWindowParameters()
            {
                Icon = lootSetup.Icon,
                Name = lootSetup.LocalizedName,
                Description = text
            };

            _previewWindowService.ShowWindow(parameters);
        }

        private void PlayAnimation()
        {
            int upgradePriceAmount = _upgradePrice.Amount;
            var parameters = new CurrencyAnimationParameters()
            {
                TextPrefix = "-",
                Type = CurrencyTypeId.Gold,
                Count = upgradePriceAmount,
                StartPosition = _gameplayCurrencyService.Holder.PlayerCurrentGold.CurrencyIcon.transform.position,
                EndPosition = UpgradePrice.CurrencyIcon.transform.position,
                EachReplenishCallback = UpgradePrice.PlayReplenish
            };

            _currencyFactory.PlayCurrencyAnimation(parameters);
        }

        private void SetAvailableButtonColors()
        {
            AvailableButton.EnableElement();
            NotAvailableButton.DisableElement();
            UpgradePrice.AmountText.color = AvailableColor;
        }

        private void SetNotAvailableButtonColors()
        {
            AvailableButton.DisableElement();
            NotAvailableButton.EnableElement();
            UpgradePrice.AmountText.color = NotAvailableColor;
        }
    }
}