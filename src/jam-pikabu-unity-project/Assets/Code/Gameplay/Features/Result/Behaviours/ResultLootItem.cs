using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Localization;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.Features.LootCollection.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Result.Behaviours
{
    public class ResultLootItem : MonoBehaviour
    {
        public TMP_Text LevelText;
        public TMP_Text AmountText;
        public Image Icon;

        private IStaticDataService _staticDataService;
        private ILootCollectionService _lootCollectionService;
        private ILocalizationService _localizationService;

        [Inject]
        private void Construct
        (
            IStaticDataService staticDataService,
            ILootCollectionService lootCollectionService,
            ILocalizationService localizationService
        )
        {
            _localizationService = localizationService;
            _lootCollectionService = lootCollectionService;
            _staticDataService = staticDataService;
        }

        public void Setup(LootTypeId type, int amount)
        {
            LootSettingsData settings = _staticDataService
                .Get<LootSettingsStaticData>()
                .GetConfig(type);

            InitLevelText(type);

            Icon.sprite = settings.Icon;
            Icon.color = Color.white;
            AmountText.text = $"x{amount.ToString()}";
        }

        private void InitLevelText(LootTypeId type)
        {
            var staticData = _staticDataService.Get<LootProgressionStaticData>();

            if (staticData.GetConfig(type) == null)
            {
                LevelText.text = string.Empty;
                return;
            }

            if (_lootCollectionService.LootLevels.TryGetValue(type, out LootLevelsProgressionData level) && level.Level > 0)
                LevelText.text = $"{_localizationService["INGREDIENTS/I_LVL"]} {(level.Level + 1).ToString()}";
            else
                LevelText.text = $"{_localizationService["INGREDIENTS/I_LVL"]} {1}";
        }
    }
}