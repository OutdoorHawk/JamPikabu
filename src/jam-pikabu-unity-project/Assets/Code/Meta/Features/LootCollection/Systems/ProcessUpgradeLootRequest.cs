using Code.Gameplay.Features.Currency.Factory;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class ProcessUpgradeLootRequest : IExecuteSystem
    {
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IGroup<MetaEntity> _lootCollection;
        private readonly IGroup<MetaEntity> _requests;
        private readonly IGroup<MetaEntity> _storages;
        private readonly ICurrencyFactory _currencyFactory;

        public ProcessUpgradeLootRequest
        (
            MetaContext context,
            ILootCollectionService lootCollectionService,
            ICurrencyFactory currencyFactory
        )
        {
            _currencyFactory = currencyFactory;
            _lootCollectionService = lootCollectionService;

            _requests = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.UpgradeLootRequest,
                MetaMatcher.LootTypeId));

            _lootCollection = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.Loot,
                MetaMatcher.LootTypeId));

            _storages = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.Gold,
                MetaMatcher.Storage));
        }

        public void Execute()
        {
            foreach (MetaEntity request in _requests)
            foreach (MetaEntity loot in _lootCollection)
            {
                if (request.LootTypeId != loot.LootTypeId)
                    continue;

                foreach (MetaEntity storage in _storages)
                {
                    request.isDestructed = true;

                    if (storage.Gold < request.Gold)
                        continue;

                    storage.ReplaceGold(storage.Gold - request.Gold);
                    ProcessUpgrade(loot);
                }
            }
        }

        private void ProcessUpgrade(MetaEntity loot)
        {
            int newLevel = loot.Level + 1;
            loot.ReplaceLevel(newLevel);
            _lootCollectionService.LootUpgraded(loot.LootTypeId, newLevel: newLevel);
        }
    }
}