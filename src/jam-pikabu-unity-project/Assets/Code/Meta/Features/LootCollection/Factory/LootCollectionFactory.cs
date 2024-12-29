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

        private LootProgressionStaticData LootData => _staticData.Get<LootProgressionStaticData>();

        public LootCollectionFactory(ITimeService timeService, IStaticDataService staticData)
        {
            _timeService = timeService;
            _staticData = staticData;
        }

        public MetaEntity CreateNewLootProgressionEntity(LootTypeId type)
        {
            MetaEntity lootMeta = CreateMetaEntity.Empty()
                .With(x => x.isLootProgression = true)
                .AddLootTypeId(type)
                .AddLevel(0);
            
            return lootMeta;
        }

        public MetaEntity CreateLootFreeUpgradeTimer(LootTypeId type, int timer)
        {
            return CreateMetaEntity.Empty()
                .With(x => x.isLootFreeUpgradeTimer = true)
                .AddLootTypeId(type)
                .AddNextFreeUpgradeTime(_timeService.TimeStamp + timer);
        }
    }
}