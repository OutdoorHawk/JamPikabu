using System.Collections.Generic;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.MapBlocks.Systems
{
    public class UpdateTimerOnFreeUpgradeLootRequestSystem : IExecuteSystem
    {
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IGroup<MetaEntity> _requests;
        private readonly IGroup<MetaEntity> _timers;
        private readonly List<MetaEntity> _buffer = new(32);

        public UpdateTimerOnFreeUpgradeLootRequestSystem(MetaContext context, ILootCollectionService lootCollectionService)
        {
            _lootCollectionService = lootCollectionService;
            _requests = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.UpgradeLootRequest,
                MetaMatcher.LootTypeId,
                MetaMatcher.FreeUpgradeRequest));
            
            _timers = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.LootFreeUpgradeTimer,
                    MetaMatcher.LootTypeId
                ));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            {
                request.isDestructed = true;
                
                foreach (var timer in _timers.GetEntities(_buffer))
                {
                    if (request.LootTypeId != timer.LootTypeId)
                        continue;

                    timer.isDestructed = true;
                    _lootCollectionService.CreateFreeUpgradeTimer(timer.LootTypeId);
                }
            }
        }
    }
}