using System;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Code.Meta.UI.Shop.Items;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Meta.UI.Shop.Configs
{
    [Serializable]
    public class ShopItemData : BaseData
    {
        public ShopItemKind Kind;
        [FoldoutGroup("Common")] public Sprite Icon;
        [FoldoutGroup("Common")] public int LayoutOrder;
        [FoldoutGroup("Common")] public LocalizedString NameLocale;
        [FoldoutGroup("Common")] public LocalizedString DescriptionLocale;
        [FoldoutGroup("Common")] public CostSetup Cost;
        [FoldoutGroup("Ingredient")] public LootTypeId IngredientType;
    }
}