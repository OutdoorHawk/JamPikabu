using System;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.View;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Gameplay.Features.Loot.Configs
{
    [Serializable]
    public class LootSetup : BaseData
    {
        public LootTypeId Type;
        [HideInInspector] public EntityView ViewPrefab;
        [PreviewField] public Sprite Icon;
        [FoldoutGroup("Data")] public float Size = 1;
        [FoldoutGroup("Data")] public bool CanBeUsedInOrders = true;
        [FoldoutGroup("Data")] public LocalizedString LocalizedName;
        [FoldoutGroup("Data")] public float ColliderSize = 0.9f;
        [FoldoutGroup("Data")] public int BaseRatingValue = 1;
    }
}