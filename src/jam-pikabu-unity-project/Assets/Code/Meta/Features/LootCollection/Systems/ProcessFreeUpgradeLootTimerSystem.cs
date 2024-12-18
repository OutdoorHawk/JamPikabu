using Code.Gameplay.Common.Time;
using Code.Infrastructure.Systems;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class ProcessFreeUpgradeLootTimerSystem : TimerExecuteSystem
    {
        private readonly ITimeService _timeService;
        private readonly ILootCollectionService _lootCollection;
        private readonly IGroup<MetaEntity> _loot;

        public ProcessFreeUpgradeLootTimerSystem(float interval, MetaContext context, ITimeService timeService, ILootCollectionService lootCollection) : base(interval, timeService)
        {
            _timeService = timeService;
            _lootCollection = lootCollection;

            _loot = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Loot,
                    MetaMatcher.LootTypeId,
                    MetaMatcher.NextFreeUpgradeTime
                ));
        }


        protected override void Execute()
        {
            foreach (var loot in _loot)
            {
                if (_timeService.TimeStamp < loot.NextFreeUpgradeTime)
                    continue;

                loot.ReplaceNextFreeUpgradeTime(_timeService.TimeStamp + loot.FreeUpgradeTimeSeconds);
                _lootCollection.FreeUpgradeTimerUpdated(loot.LootTypeId, loot.NextFreeUpgradeTime);
            }
        }
    }
}