using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Meta.Features.DayLootSettings.Configs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MapBlocks.Behaviours
{
    public class AvailableIngredientsView : MonoBehaviour
    {
        public HorizontalLayoutGroup Layout;
        public Image IconTemplate;
        private IStaticDataService _staticData;

        private LootSettingsStaticData LootSettings => _staticData.Get<LootSettingsStaticData>();

        [Inject]
        private void Construct(IStaticDataService staticData)
        {
            _staticData = staticData;
        }

        public void Init(MapBlockData mapBlockData)
        {
            foreach (var type in mapBlockData.AvailableIngredients)
            {
                LootSetup lootSetup = LootSettings.GetConfig(type);
                
                if (lootSetup.CanBeUsedInOrders== false)
                    continue;
                
                Image instance = Instantiate(IconTemplate, Layout.transform);
                instance.sprite = lootSetup.Icon;
            }
        }
    }
}