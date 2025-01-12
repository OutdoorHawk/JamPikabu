using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData;
using Code.Gameplay.Tutorial.Behaviours;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.UI.Shop.Configs;
using Code.Progress.Provider;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Behaviours
{
    public class ShopWindowButton : MonoBehaviour
    {
        public GameObject Pin;
        public Button ShopButton;
        public TutorialConditionComponent TutorialConditionComponent;

        private IGameplayCurrencyService _currencyService;
        private ILootCollectionService _lootCollectionService;
        private IStaticDataService _staticData;
        private IProgressProvider _progress;

        [Inject]
        private void Construct(IGameplayCurrencyService currencyService, ILootCollectionService lootCollectionService, IStaticDataService staticData,
            IProgressProvider progress)
        {
            _progress = progress;
            _staticData = staticData;
            _lootCollectionService = lootCollectionService;
            _currencyService = currencyService;
        }

        private void Awake()
        {
            ShopButton.onClick.AddListener(DisablePin);
            TutorialConditionComponent.ConditionChanged += RefreshPin;
        }

        private void Start()
        {
            RefreshPin();
        }

        private void OnDestroy()
        {
            ShopButton.onClick.RemoveListener(DisablePin);
            TutorialConditionComponent.ConditionChanged -= RefreshPin;
        }

        private void DisablePin()
        {
            Pin.DisableElement();
        }

        private void RefreshPin()
        {
            Pin.DisableElement();

            if (TutorialConditionComponent.CurrentCondition == false)
                return;

            foreach ((LootTypeId key, LootLevelsProgressionData value) in _lootCollectionService.LootLevels)
            {
                if (_lootCollectionService.UpgradedForMaxLevel(key))
                    continue;

                LootProgressionData progressionData = _lootCollectionService.LootData.GetConfig(key);
                int nextLevel = value.Level + 1;

                if (nextLevel >= progressionData.Levels.Count)
                    continue;

                LootLevelData level = progressionData.Levels[nextLevel];

                int amount = _currencyService.GetCurrencyOfType(level.Cost.CurrencyType);

                if (amount >= level.Cost.Amount)
                {
                    Pin.EnableElement();
                    return;
                }
            }

            foreach (var data in _staticData.Get<ShopStaticData>().Configs)
            {
                if (_currencyService.GetCurrencyOfType(data.Cost.CurrencyType, false) >= data.Cost.Amount)
                {
                    Pin.EnableElement();
                    return;
                }
            }

            if (_progress.Progress.Tutorial.ConsumablesPinSeen == false)
            {
                Pin.EnableElement();
                return;
            }
        }
    }
}