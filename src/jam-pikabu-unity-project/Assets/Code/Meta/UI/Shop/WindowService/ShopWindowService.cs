using System;
using Code.Gameplay.StaticData;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.Window;

namespace Code.Meta.UI.Shop.WindowService
{
    public class ShopWindowService : IShopWindowService
    {
        private readonly IStaticDataService _staticDataService;
        public event Action OnSelectionChanged;
        public ShopTabTypeId SelectedTab { get; private set; }

        private ShopStaticData DaysStaticData => _staticDataService.GetStaticData<ShopStaticData>();
        
        private ShopItemTemplatesStaticData TemplatesStaticData => _staticDataService.GetStaticData<ShopItemTemplatesStaticData>();

        public ShopWindowService(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public void SetTabSelected(ShopTabTypeId type)
        {
            if (SelectedTab == type)
                return;
            
            SelectedTab = type;
            NotifyChanged();
        }

        public ShopItemTemplateData GetTemplate(ShopItemKind kind)
        {
            return TemplatesStaticData.GetByItemKind(kind);
        }

        private void NotifyChanged()
        {
            OnSelectionChanged?.Invoke();
        }
    }
}