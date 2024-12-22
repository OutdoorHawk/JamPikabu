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

        private readonly Dictionary<ShopTabTypeId, BaseShopTab> _tabsDictionary = new();

        private IShopWindowService _shopWindowService;

        public IReadOnlyDictionary<ShopTabTypeId, BaseShopTab> TabsDictionary => _tabsDictionary;

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
            _tabsDictionary.Clear();

            foreach (var tab in TabsParent.GetComponentsInChildren<BaseShopTab>())
                _tabsDictionary[tab.TypeId] = tab;

            _shopWindowService.SetTabSelected(ShopTabTypeId.LootUpgrade);
            Refresh();
        }

        public BaseShopTab GetTab(ShopTabTypeId id)
        {
            return _tabsDictionary.GetValueOrDefault(id);
        }

        private void Refresh()
        {
            UpdateTabs();
        }

        private void UpdateTabs()
        {
            _tabsDictionary[_shopWindowService.SelectedTab].ActivateTab();

            foreach (BaseShopTab baseShopTab in _tabsDictionary.Values)
            {
                if (baseShopTab.TypeId != _shopWindowService.SelectedTab)
                    baseShopTab.DeactivateTab();
            }
        }
    }
}