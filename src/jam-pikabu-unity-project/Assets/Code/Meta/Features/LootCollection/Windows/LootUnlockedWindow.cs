using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using TMPro;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.LootCollection.Windows
{
    public class LootUnlockedWindow : BaseWindow
    {
        public TMP_Text Name;
        public TMP_Text Description;
        public Image Icon;
        
        private IStaticDataService _staticData;

        [Inject]
        private void Construct(IStaticDataService staticData)
        {
            _staticData = staticData;
        }

        public void Init(LootTypeId lootTypeId)
        {
            LootSettingsData data = _staticData.Get<LootSettingsStaticData>().GetConfig(lootTypeId);
            Icon.sprite = data.Icon;
            Description.text = data.LocalizedDescription.GetLocalizedString();
            Name.text = data.LocalizedName.GetLocalizedString();
        }
    }
}