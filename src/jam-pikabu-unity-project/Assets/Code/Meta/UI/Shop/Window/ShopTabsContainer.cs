using System.Collections.Generic;
using Code.Meta.UI.Shop.Templates;
using Code.Meta.UI.Shop.WindowService;
using UnityEngine;
using Zenject;

namespace Code.Meta.UI.Shop.Window
{
    public class ShopTabsContainer : MonoBehaviour
    {
        public RectTransform TabsParent;

        private readonly Dictionary<ShopTabTypeId, BaseShopTab> TabsDictionary = new();

        private IShopWindowService _shopWindowService;

        [Inject]
        private void Construct(IShopWindowService shopWindowService)
        {
            _shopWindowService = shopWindowService;
        }

        private void Start()
        {
            _shopWindowService.OnSelectionChanged += Refresh;
        }

        private void OnDestroy()
        {
            _shopWindowService.OnSelectionChanged -= Refresh;
        }

        public void Init()
        {
            TabsDictionary.Clear();

            foreach (var tab in TabsParent.GetComponentsInChildren<BaseShopTab>())
                TabsDictionary[tab.TypeId] = tab;

            _shopWindowService.SetTabSelected(ShopTabTypeId.LootUpgrade);
            Refresh();
        }

        public BaseShopTab GetTab(ShopTabTypeId id)
        {
            return TabsDictionary.GetValueOrDefault(id);
        }

        private void Refresh()
        {
            UpdateTabs();
        }

        private void UpdateTabs()
        {
            TabsDictionary[_shopWindowService.SelectedTab].ActivateTab();

            foreach (BaseShopTab baseShopTab in TabsDictionary.Values)
            {
                if (baseShopTab.TypeId != _shopWindowService.SelectedTab)
                    baseShopTab.DeactivateTab();
            }
        }
    }
}