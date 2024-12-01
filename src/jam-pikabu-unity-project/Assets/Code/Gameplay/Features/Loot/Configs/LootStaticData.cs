using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(LootStaticData), fileName = "Loot")]
    public class LootStaticData : BaseStaticData<LootSetup>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();

            AddIndex(setup => (int)setup.Type);
        }

        public LootSetup GetConfig(LootTypeId typeId)
        {
            return GetByKey((int)typeId);
        }
    }
}