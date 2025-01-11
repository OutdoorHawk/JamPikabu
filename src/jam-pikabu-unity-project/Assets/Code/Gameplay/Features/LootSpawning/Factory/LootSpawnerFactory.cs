using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;

namespace Code.Gameplay.Features.LootSpawning.Factory
{
    public class LootSpawnerFactory : ILootSpawnerFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;

        public LootSpawnerFactory(IStaticDataService staticDataService, IDaysService daysService)
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
        }

        public GameEntity CreateLootSpawner()
        {
            DayData dayData = _daysService.GetDayData();

            switch (dayData.SceneId)
            {
                case SceneTypeId.DefaultGameplayScene:
                    return CreateDefaultSpawner();
                case SceneTypeId.ConveyorGameplayScene:
                    return CreateConveyorSpawner();
                case SceneTypeId.BounceGameplayScene when _daysService.BonusLevelType != BonusLevelType.None:
                    return CreateBonusLevelSpawner();
                default:
                    return CreateDefaultSpawner();
            }
        }

        private GameEntity CreateDefaultSpawner()
        {
            var settings = _staticDataService.Get<LootSettingsStaticData>();

            return CreateGameEntity.Empty()
                    .With(x => x.isLootSpawner = true)
                    .With(x => x.isContinuousSpawn = true)
                    .With(x => x.isCooldownUp = true)
                    .With(x => x.isComplete = true)
                    .AddLootSpawnInterval(settings.LootSpawnInterval)
                ;
        }

        private GameEntity CreateConveyorSpawner()
        {
            var settings = _staticDataService.Get<LootSettingsStaticData>();

            return CreateGameEntity.Empty()
                    .With(x => x.isLootSpawner = true)
                    .With(x => x.isConveyorSpawner = true)
                    .With(x => x.isCooldownUp = true)
                    .With(x => x.isComplete = true)
                    .AddLootSpawnInterval(settings.LootSpawnConveyorInterval)
                ;
        }

        private GameEntity CreateBonusLevelSpawner()
        {
            var settings = _staticDataService.Get<LootSettingsStaticData>();

            return CreateGameEntity.Empty()
                    .With(x => x.isLootSpawner = true)
                    .With(x => x.isBonusLevelSpawn = true)
                    .With(x => x.isCooldownUp = true)
                    .With(x => x.isComplete = true)
                    .AddLootSpawnInterval(settings.LootSpawnInterval)
                ;
        }
    }
}