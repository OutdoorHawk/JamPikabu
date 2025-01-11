using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.StaticData;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.Service;
using Entitas;

namespace Code.Meta.UI.Shop.Systems
{
    public class BuyItemOnRequestSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _shopItemPurchaseRequests;
        private readonly IGroup<MetaEntity> _goldStorages;
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

            _goldStorages = meta.GetGroup(MetaMatcher
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
            foreach (MetaEntity request in _shopItemPurchaseRequests)
            {
                request.isDestructed = true;

                ShopItemData data = _staticDataService.Get<ShopStaticData>().GetById(request.ShopItemId);

                if (TryPurchase(data) == false)
                    continue;

                CreateMetaEntity.Empty()
                    .AddShopItemId(request.ShopItemId)
                    .With(x => x.isConsumable = true, when: data.Consumable)
                    .isPurchased = true;
            }
        }

        private bool TryPurchase(ShopItemData data)
        {
            switch (data.Cost.CurrencyType)
            {
                case CurrencyTypeId.Gold:
                {
                    foreach (MetaEntity storage in _goldStorages)
                    {
                        if (storage.Gold < data.Cost.Amount)
                            continue;

                        storage.ReplaceGold(storage.Gold - data.Cost.Amount);
                        return true;
                    }

                    break;
                }
            }

            return false;
        }
    }
}