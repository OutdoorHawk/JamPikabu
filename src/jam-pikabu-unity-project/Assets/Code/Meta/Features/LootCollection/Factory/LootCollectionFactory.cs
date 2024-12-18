using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData;
using Code.Meta.Features.LootCollection.Configs;

namespace Code.Meta.Features.LootCollection.Factory
{
    public class LootCollectionFactory : ILootCollectionFactory
    {
        private readonly ITimeService _timeService;
        private readonly IStaticDataService _staticData;

        private LootProgressionStaticData LootData => _staticData.GetStaticData<LootProgressionStaticData>();

        public LootCollectionFactory(ITimeService timeService, IStaticDataService staticData)
        {
            _timeService = timeService;
            _staticData = staticData;
        }

        public MetaEntity UnlockNewLoot(LootTypeId type)
        {
            MetaEntity lootMeta = CreateMetaEntity.Empty()
                .With(x => x.isLoot = true)
                .AddLootTypeId(type)
                .AddLevel(0);


            AddFreeUpgradeTime(lootMeta);
            return lootMeta;
        }

        private void AddFreeUpgradeTime(MetaEntity lootMeta)
        {
            LootProgressionData data = LootData.GetConfig(lootMeta.LootTypeId);

            if (data.FreeUpgradeTimeHours == 0)
                return;

            int timeSeconds = (int)(data.FreeUpgradeTimeHours * 60 * 60);
            int nextTime = _timeService.TimeStamp + timeSeconds;
            
            lootMeta.AddNextFreeUpgradeTime(nextTime)
                .AddFreeUpgradeTimeSeconds(timeSeconds)
                ;
        }
    }
}