using System.Collections.Generic;
using Code.Meta.Features.Consumables.Behaviours;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.Service;
using UnityEngine;
using Zenject;

namespace Code.Meta.UI.Shop.Tabs
{
    public class ConsumablesShopTab : BaseShopTab
    {
        public Transform Content;

        private IShopUIService _shopUIService;

        private ShopStaticData ShopStaticData => _staticData.Get<ShopStaticData>();

        [Inject]
        private void Construct
        (
            IShopUIService shopUIService
        )
        {
            _shopUIService = shopUIService;
        }

        private void Start()
        {
            CreateConsumables();
        }

        public override void ActivateTab()
        {
            base.ActivateTab();
        }

        private void CreateConsumables()
        {
            List<ShopItemData> items = ShopStaticData.GetByItemKind(ShopItemKind.ExtraLoot);
            ShopItemTemplateData template = _shopWindowService.GetTemplate(ShopItemKind.ExtraLoot);

            items.Sort((x, y) => x.LayoutOrder.CompareTo(y.LayoutOrder));

            foreach (ShopItemData shopItemData in items)
            {
                GameObject prefab = _instantiator.InstantiatePrefab(template.Prefab, Content);
                prefab.GetComponent<ConsumableShopItem>().Initialize(shopItemData);
            }
        }
    }
}