using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Meta.Features.LootCollection;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.WindowService;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Templates.UpgradeLoot
{
    public class LootUpgradeShopTab : BaseShopTab
    {
        public GridLayoutGroup Layout;

        private readonly List<LootUpgradeShopItem> _items = new();

        private IShopWindowService _shopWindowService;
        private IStaticDataService _staticDataService;
        private ILootCollectionService _lootCollectionService;
        private IInstantiator _instantiator;

        [Inject]
        private void Construct
        (
            IShopWindowService shopWindowService,
            IStaticDataService staticDataService,
            ILootCollectionService lootCollectionService,
            IInstantiator instantiator
        )
        {
            _instantiator = instantiator;
            _lootCollectionService = lootCollectionService;
            _staticDataService = staticDataService;
            _shopWindowService = shopWindowService;
        }

        private void Awake()
        {
            _lootCollectionService.OnUpgraded += Refresh;
        }

        private void OnDestroy()
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

            gameObject.EnableElement();
        }

        private void CreateItems()
        {
            ShopItemTemplateData prefab = _shopWindowService.GetTemplate(ShopItemKind.UpgradeIngredient);

            foreach (LootItemCollectionData item in _lootCollectionService.LootProgression.Values)
            {
                var instance = _instantiator.InstantiatePrefabForComponent<GameObject>(prefab.Prefab, Layout.transform);
                var upgradeItem = instance.GetComponent<LootUpgradeShopItem>();
                upgradeItem.Init(in item);
                _items.Add(upgradeItem);
            }
        }

        private void Refresh()
        {
            foreach (LootUpgradeShopItem item in _items)
            {
                item.Init(_lootCollectionService.LootProgression[item.Type]);
            }
        }
    }
}