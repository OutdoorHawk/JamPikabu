using System;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [Serializable]
    public class LootSetup
    {
        public LootTypeId Type;
        public EntityView ViewPrefab;
        public Sprite Icon;
        public int MinDayToUnlock;
        public int MaxDayToUnlock;
    }
}