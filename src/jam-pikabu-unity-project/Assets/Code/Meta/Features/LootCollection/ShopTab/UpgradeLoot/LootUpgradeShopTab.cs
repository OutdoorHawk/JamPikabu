using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.Tabs;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.LootCollection.ShopTab.UpgradeLoot
{
    public class LootUpgradeShopTab : BaseShopTab
    {
        public GridLayoutGroup Layout;

        private readonly List<LootUpgradeShopItem> _items = new();

        private ILootCollectionService _lootCollectionService;

        public IReadOnlyList<LootUpgradeShopItem> Items => _items;

        [Inject]
        private void Construct
        (
            ILootCollectionService lootCollectionService
        )
        {
            _lootCollectionService = lootCollectionService;
        }

        private void OnEnable()
        {
            _lootCollectionService.OnUpgraded += Refresh;
        }

        private void OnDisable()
        {
            _lootCollectionService.OnUpgraded -= Refresh;
        }

        private void Start()
        {
            CreateItems();
        }

        public override void ActivateTab()
        {
            base.ActivateTab();
        }

        private void CreateItems()
        {
            ShopItemTemplateData prefab = _shopWindowService.GetTemplate(ShopItemKind.UpgradeIngredient);

            foreach (LootLevelsProgressionData item in _lootCollectionService.LootLevels.Values)
            {
                var instance = _instantiator.InstantiatePrefab(prefab.Prefab, Layout.transform);
                var upgradeItem = instance.GetComponent<LootUpgradeShopItem>();
                upgradeItem.Init(in item);
                _items.Add(upgradeItem);
            }
        }

        private void Refresh()
        {
            foreach (LootUpgradeShopItem item in _items)
            {
                item.Init(_lootCollectionService.LootLevels[item.Type]);
            }
        }
    }
}