using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;

namespace Code.Meta.UI.LootProgression.Configs
{
    [Serializable]
    public class LootProgressionData : BaseData
    {
        public LootTypeId Type;
        public List<LootLevelData> Levels;
    }
}