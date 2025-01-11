using System;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Meta.UI.Shop.Configs
{
    [Serializable]
    public class ShopItemData : BaseData
    {
        public ShopItemKind Kind;
        [FoldoutGroup("Data")] public Sprite Icon;
        [FoldoutGroup("Data")] public int LayoutOrder;
        [FoldoutGroup("Data")] public LocalizedString NameLocale;
        [FoldoutGroup("Data")] public LocalizedString DescriptionLocale;
        [FoldoutGroup("Data")] public CostSetup Cost;
        
        [FoldoutGroup("Data")] public bool Consumable;
        [FoldoutGroup("Data")] public LootTypeId LootType;
        [FoldoutGroup("Data")] public int MinutesDuration;
    }
}