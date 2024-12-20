using System;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.Loot;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.Features.LootCollection.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Behaviours
{
    public class ShopWindowButton : MonoBehaviour
    {
        public GameObject Pin;
        public Button ShopButton;
        private IGameplayCurrencyService _currencyService;
        private ILootCollectionService _lootCollectionService;

        [Inject]
        private void Construct(IGameplayCurrencyService currencyService, ILootCollectionService lootCollectionService)
        {
            _lootCollectionService = lootCollectionService;
            _currencyService = currencyService;
        }

        private void Awake()
        {
            ShopButton.onClick.AddListener(DisablePin);
        }

        private void Start()
        {
            RefreshPin();
        }

        private void OnDestroy()
        {
            ShopButton.onClick.RemoveListener(DisablePin);
        }

        private void DisablePin()
        {
            Pin.DisableElement();
        }

        private void RefreshPin()
        {
            Pin.DisableElement();
            
            foreach ((LootTypeId key, LootLevelsProgressionData value) in _lootCollectionService.LootLevels)
            {
                if (_lootCollectionService.UpgradedForMaxLevel(key))
                    continue;

                LootProgressionData progressionData = _lootCollectionService.LootData.GetConfig(key);
                LootLevelData level = progressionData.Levels[value.Level];

                int amount = _currencyService.GetCurrencyOfType(level.Cost.CurrencyType);

                if (amount >= level.Cost.Amount)
                {
                    Pin.EnableElement();
                    return;
                }
            }
        }
    }
}