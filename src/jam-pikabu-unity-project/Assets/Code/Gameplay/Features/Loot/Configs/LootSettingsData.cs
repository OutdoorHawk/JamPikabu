using System;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.View;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Gameplay.Features.Loot.Configs
{
    [Serializable]
    public class LootSettingsData : BaseData
    {
        public LootTypeId Type;
        public EntityView ViewPrefab;
        [PreviewField] public Sprite Icon;
        [FoldoutGroup("Data")] public bool CanBeUsedInOrders = true;
        [FoldoutGroup("Data")] public LocalizedString LocalizedName;
        [FoldoutGroup("Data")] public LocalizedString LocalizedDescription;
        [FoldoutGroup("Data")] public int BaseRatingValue = 1;
        [FoldoutGroup("Data")] public float EffectValue;
        [FoldoutGroup("Data"), Range(0, 100)] public int SpawnChance = 100;
        [FoldoutGroup("Data")] public AbilityTypeId AbilityType;
    }
}