using Code.Gameplay.StaticData;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(GrapplingHookStaticData), fileName = "GrapplingHook")]
    public class GrapplingHookStaticData : BaseStaticData
    {
        public float XAxisSpeed = 1;
        public float YAxisDownSpeed = 1;
        public float YAxisUpSpeed = 1;
        public float StopMovementRaycastRadius = 2;
        public float CollectLootRaycastRadius = 1;
        public float CollectLootPieceInterval = 0.15f;
        public float TriggerMovementThreshold = 0.5f;
      
        public Vector2 XMovementLimits = new(3.5f, 3.5f);
        
        public EntityView ViewPrefab;
    }
}