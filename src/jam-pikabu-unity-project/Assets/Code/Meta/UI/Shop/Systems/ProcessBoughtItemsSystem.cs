using System.Collections.Generic;
using Code.Meta.UI.Shop.Factory;
using Code.Meta.UI.Shop.Service;
using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Meta.UI.Shop.Systems
{
    public class ProcessBoughtItemsSystem : ReactiveSystem<MetaEntity>
    {
        private readonly IShopItemFactory _shopItemFactory;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IShopUIService _shopUIService;

        public ProcessBoughtItemsSystem(MetaContext meta, IShopItemFactory shopItemFactory,
            ISaveLoadService saveLoadService, IShopUIService shopUIService) : base(meta)
        {
            _saveLoadService = saveLoadService;
            _shopUIService = shopUIService;
            _shopItemFactory = shopItemFactory;
        }

        protected override ICollector<MetaEntity> GetTrigger(IContext<MetaEntity> context) =>
            context.CreateCollector(MetaMatcher.Purchased.Added());

        protected override bool Filter(MetaEntity purchases) => purchases.hasShopItemId;

        protected override void Execute(List<MetaEntity> purchases)
        {
            foreach (MetaEntity purchase in purchases)
            {
                _shopItemFactory.CreateShopItem(purchase.ShopItemId, purchase.isForAd);

                if (purchase.isConsumable)
                    purchase.isDestructed = true;
            }
            
            _shopUIService.NotifyPurchase();
            //_saveLoadService.SaveProgress();
        }
    }
}