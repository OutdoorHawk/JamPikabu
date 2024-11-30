using Code.Common.Entity;
using Code.Common.Extensions;
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
            var config = _staticDataService.GetStaticData<GrapplingHookStaticData>();

            return CreateGameEntity
                    .Empty()
                    .With(x => x.isGrapplingHook = true)
                    .AddXAxisSpeed(config.XAxisSpeed)
                    .AddYAxisDownSpeed(config.YAxisDownSpeed)
                    .AddYAxisUpSpeed(config.YAxisUpSpeed)
                    .AddStopMovementRaycastRadius(config.StopMovementRaycastRadius)
                    .AddCollectLootRaycastRadius(config.CollectLootRaycastRadius)
                    .AddXMovementLimits(config.XMovementLimits)
                    .AddViewPrefab(config.ViewPrefab)
                    .AddTargetParent(parent)
                ;
        }
    }
}