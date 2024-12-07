using System;
using Code.Infrastructure.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [Serializable]
    public class LootSetup
    {
        public LootTypeId Type;
        [HideInInspector] public EntityView ViewPrefab;
        [PreviewField] public Sprite Icon;
        public int MinDayToUnlock;
        public int MaxDayToUnlock;
    }
}