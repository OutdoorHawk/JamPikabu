using Code.Gameplay.StaticData;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(GrapplingHookStaticData), fileName = "GrapplingHook")]
    public class GrapplingHookStaticData : BaseStaticData
    {
        public float XAxisSpeed = 1;
        public float YAxisSpeed = 1;
        public EntityView ViewPrefab;
    }
}