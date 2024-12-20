using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Systems;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.MapBlocks.Systems
{
    public class ProcessFreeUpgradeLootTimerSystem : TimerExecuteSystem
    {
        private readonly ITimeService _timeService;
        private readonly ILootCollectionService _lootCollection;
        private readonly IGroup<MetaEntity> _timers;
        private readonly List<MetaEntity> _buffer = new(32);

        public ProcessFreeUpgradeLootTimerSystem(float interval, MetaContext context,
            ITimeService timeService, ILootCollectionService lootCollection) : base(interval, timeService)
        {
            _timeService = timeService;
            _lootCollection = lootCollection;

            _timers = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.LootFreeUpgradeTimer,
                    MetaMatcher.LootTypeId,
                    MetaMatcher.NextFreeUpgradeTime
                ).NoneOf(
                    MetaMatcher.ReadyToFreeUpgrade));
        }


        protected override void Execute()
        {
            foreach (var loot in _timers.GetEntities(_buffer))
            {
                if (_timeService.TimeStamp < loot.NextFreeUpgradeTime)
                    continue;

                loot.isReadyToFreeUpgrade = true;
                _lootCollection.FreeUpgradeTimerEnded(loot.LootTypeId);
            }
        }
    }
}