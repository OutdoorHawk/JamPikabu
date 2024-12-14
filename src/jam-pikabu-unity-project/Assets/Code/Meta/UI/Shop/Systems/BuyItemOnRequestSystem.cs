using Code.Gameplay.StaticData;
using Code.Meta.UI.Shop.Service;
using Entitas;

namespace Code.Meta.UI.Shop.Systems
{
    public class BuyItemOnRequestSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _shopItemPurchaseRequests;
        private readonly IGroup<MetaEntity> _storages;
        private readonly IStaticDataService _staticDataService;
        private readonly IShopUIService _shopUIService;

        public BuyItemOnRequestSystem
        (
            MetaContext meta,
            IStaticDataService staticDataService,
            IShopUIService shopUIService
        )
        {
            _shopUIService = shopUIService;
            _staticDataService = staticDataService;

            _storages = meta.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Storage,
                    MetaMatcher.Gold));

            _shopItemPurchaseRequests = meta.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.BuyRequest,
                    MetaMatcher.ShopItemId));
        }

        public void Execute()
        {
            foreach (MetaEntity storage in _storages)
            foreach (MetaEntity request in _shopItemPurchaseRequests)
            {
                request.isDestructed = true;

                if (_shopUIService.IsItemPurchased(request.ShopItemId))
                    continue;

                /*ShopItemConfig config = _staticDataService.GetShopItemConfig(request.ShopItemId);
                ShopItemSetup shopItemSetup = config.Data;

                CreateMetaEntity.Empty()
                    .AddShopItemId(request.ShopItemId)
                    .isPurchased = true;

                _shopUIService.UpdatePurchasedItem(request.ShopItemId);*/
            }
        }
    }
}