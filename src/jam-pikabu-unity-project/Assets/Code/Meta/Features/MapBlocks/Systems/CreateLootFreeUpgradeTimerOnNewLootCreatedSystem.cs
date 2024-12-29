using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.LootCollection.Factory;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.MapBlocks.Systems
{
    public class CreateLootFreeUpgradeTimerOnNewLootCreatedSystem : IExecuteSystem
    {
        private readonly ILootCollectionService _lootCollection;
        private readonly IStaticDataService _staticDataService;
        private readonly ILootCollectionFactory _lootCollectionFactory;
        private readonly IGroup<MetaEntity> _request;

        private MapBlocksStaticData BlocksStaticData => _staticDataService.Get<MapBlocksStaticData>();

        public CreateLootFreeUpgradeTimerOnNewLootCreatedSystem
        (
            MetaContext context,
            ILootCollectionService lootCollection,
            IStaticDataService staticDataService,
            ILootCollectionFactory lootCollectionFactory
        )
        {
            _lootCollection = lootCollection;
            _staticDataService = staticDataService;
            _lootCollectionFactory = lootCollectionFactory;

            _request = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.UnlockLootRequest,
                    MetaMatcher.LootTypeId
                ));
        }

        public void Execute()
        {
            foreach (var request in _request)
            {
                LootTypeId lootTypeId = request.LootTypeId;

                if (_lootCollection.CanUpgradeForFree(lootTypeId) == false)
                    continue;
                
                _lootCollection.CreateFreeUpgradeTimer(type: lootTypeId);
            }
        }
    }
}