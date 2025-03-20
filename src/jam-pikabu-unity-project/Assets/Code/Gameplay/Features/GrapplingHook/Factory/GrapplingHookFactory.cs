using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.CharacterStats;
using Code.Gameplay.Features.GrapplingHook.Configs;
using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Factory
{
    public class GrapplingHookFactory : IGrapplingHookFactory
    {
        private readonly IStaticDataService _staticDataService;

        public GrapplingHookFactory(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public GameEntity CreateGrapplingHook(Transform parent)
        {
            var config = _staticDataService.Get<GrapplingHookStaticData>();
            
            Dictionary<Stats,float> baseStats = InitStats.EmptyStatDictionary()
                .With(x => x[Stats.Speed] = 1)
                .With(x => x[Stats.Scale] = 1)
                ;

            return CreateGameEntity
                    .Empty()
                    .With(x => x.isGrapplingHook = true)
                    .With(x => x.isXAxisMovementAvailable = true)
                    .With(x => x.isDescentAvailable = true)
                    .AddXAxisSpeed(config.XAxisSpeed)
                    .AddYAxisDownSpeed(config.YAxisDownSpeed)
                    .AddYAxisUpSpeed(config.YAxisUpSpeed)
                    .AddSpeed(baseStats[Stats.Speed])
                    .AddScale(baseStats[Stats.Scale])
                    .AddHookSpeedModifier(1)
                    .AddStopMovementRaycastRadius(config.StopMovementRaycastRadius)
                    .AddCollectLootRaycastRadius(config.CollectLootRaycastRadius)
                    .AddCollectLootPieceInterval(config.CollectLootPieceInterval)
                    .AddXMovementLimits(config.XMovementLimits)
                    .AddTriggerMovementThreshold(config.TriggerMovementThreshold)
                    .AddBaseStats(baseStats)
                    .AddStatModifiers(InitStats.EmptyStatDictionary())
                ;
        }
    }
}