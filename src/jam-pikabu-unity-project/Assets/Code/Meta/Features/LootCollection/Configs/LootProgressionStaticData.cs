using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.Features.LootCollection.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(LootProgressionStaticData), fileName = "LootProgression")]
    public class LootProgressionStaticData : BaseStaticData<LootProgressionData>
    {
        public List<LootTypeId> StartGameUnlockedLoot;
        
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