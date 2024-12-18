using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;

namespace Code.Meta.Features.LootCollection.Configs
{
    [Serializable]
    public class LootProgressionData : BaseData
    {
        public LootTypeId Type;
        public float FreeUpgradeTimeHours;
        public List<LootLevelData> Levels;
    }
}