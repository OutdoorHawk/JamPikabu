using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Meta.Features.LootCollection;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Templates.UpgradeLoot
{
    public class LootUpgradeShopItem : MonoBehaviour
    {
        public Image Icon;
        public TMP_Text Name;
        public Button UpgradeButton;
        public PriceInfo UpgradePrice;
        public PriceInfo RatingFrom;
        public PriceInfo RatingTo;

        private IStaticDataService _staticData;
        private IGameplayCurrencyService _gameplayCurrencyService;
        
        private CostSetup _upgradePrice;

        public LootTypeId Type { get; private set; }
        private LootSettingsStaticData LootSettings => _staticData.GetStaticData<LootSettingsStaticData>();
        private LootProgressionStaticData ProgressionStaticData => _staticData.GetStaticData<LootProgressionStaticData>();
        
        [Inject]
        private void Construct(IStaticDataService staticData, IGameplayCurrencyService gameplayCurrencyService)
        {
            _gameplayCurrencyService = gameplayCurrencyService;
            _staticData = staticData;
        }

        private void Start()
        {
            _gameplayCurrencyService.CurrencyChanged += RefreshState;
            UpgradeButton.onClick.AddListener(PurchaseUpgrade);
        }

        private void OnDestroy()
        {
            _gameplayCurrencyService.CurrencyChanged -= RefreshState;
            UpgradeButton.onClick.RemoveListener(PurchaseUpgrade);
        }

        public void Init(in LootItemCollectionData item)
        {
            Icon.sprite = LootSettings.GetConfig(item.Type).Icon;
            Name.text = LootSettings.GetConfig(item.Type).LocalizedName.GetLocalizedString();
            Type = item.Type;
            InitCurrentLevel(in item);
            InitNextLevel(in item);
            InitUpgradePrice(in item);
            UpdateAvailable();
        }

        private void InitCurrentLevel(in LootItemCollectionData item)
        {
            List<LootLevelData> levels = ProgressionStaticData.GetConfig(item.Type).Levels;

            if (item.Level >= levels.Count)
                return;

            LootLevelData currentLevel = levels[item.Level];
            RatingFrom.SetupPrice(currentLevel.RatingBoostAmount, CurrencyTypeId.Plus);
        }

        private void InitNextLevel(in LootItemCollectionData item)
        {
            List<LootLevelData> levels = ProgressionStaticData.GetConfig(item.Type).Levels;

            if (item.Level + 1 >= levels.Count)
            {
                InitMaxLevelReached();
                return;
            }

            LootLevelData nextLevel = levels[item.Level + 1];
            RatingTo.SetupPrice(nextLevel.RatingBoostAmount, CurrencyTypeId.Plus);
        }

        private void InitUpgradePrice(in LootItemCollectionData item)
        {
            _upgradePrice = null;
            
            List<LootLevelData> levels = ProgressionStaticData.GetConfig(item.Type).Levels;

            if (item.Level >= levels.Count)
                return;

            LootLevelData currentLevel = levels[item.Level];
            UpgradePrice.SetupPrice(currentLevel.Cost);
            _upgradePrice = currentLevel.Cost;
        }

        private void InitMaxLevelReached()
        {
            UpgradeButton.interactable = false;
            RatingTo.DisableElement();
        }

        private void UpdateAvailable()
        {
            if (_upgradePrice == null)
            {
                SetUpgradeLocked();
                return;
            }
            
            int goldAmount = _gameplayCurrencyService.GetCurrencyOfType(_upgradePrice.CurrencyType);
            
            if (goldAmount >= _upgradePrice.Amount)
                SetCanUpgrade();
            else
                SetUpgradeLocked();
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
        }

        private void SetCanUpgrade()
        {
            UpgradeButton.interactable = true;
        }

        private void SetUpgradeLocked()
        {
            UpgradeButton.interactable = false;
        }
    }
}