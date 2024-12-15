using System;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [Serializable]
    public class LootSetup : BaseData
    {
        public LootTypeId Type;
        [HideInInspector] public EntityView ViewPrefab;
        [PreviewField] public Sprite Icon;
        public int MinDayToUnlock;
        public int MaxDayToUnlock;
        public float Size = 1;
        public float ColliderSize = 0.9f;
        public int BaseRatingValue = 1;
    }
}