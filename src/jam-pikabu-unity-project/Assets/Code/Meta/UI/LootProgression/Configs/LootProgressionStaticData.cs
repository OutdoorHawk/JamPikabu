using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.UI.LootProgression.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(LootProgressionStaticData), fileName = "LootProgression")]
    public class LootProgressionStaticData : BaseStaticData<LootProgressionData>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();

            AddIndex(setup => (int)setup.Type);
        }

        public LootProgressionData GetConfig(LootTypeId typeId)
        {
            return GetByKey((int)typeId);
        }
    }
}