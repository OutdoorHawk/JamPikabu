using System;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.Window;

namespace Code.Meta.UI.Shop.WindowService
{
    public interface IShopWindowService
    {
        event Action OnSelectionChanged;
        ShopTabTypeId SelectedTab { get; }
        void SetTabSelected(ShopTabTypeId type);
        ShopItemTemplateData GetTemplate(ShopItemKind kind);
    }
}