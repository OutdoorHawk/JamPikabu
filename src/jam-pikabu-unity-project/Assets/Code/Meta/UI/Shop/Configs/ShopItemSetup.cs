using System;
using Code.Meta.UI.Shop.Items;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Meta.UI.Shop.Configs
{
    [Serializable]
    public class ShopItemSetup
    {
        public int ShopItemId;
        public ShopItemKind Kind;
        public Sprite Icon;
        public int LayoutOrder;
        public LocalizedString NameLocale;
        public LocalizedString DescriptionLocale;
    }
}