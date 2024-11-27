using System;
using Sirenix.OdinInspector;

namespace Code.Meta.UI.Shop.Configs
{
    [Serializable]
    public class ShopItemConfig
    {
       [HideLabel] public ShopItemSetup Data;
    }
}