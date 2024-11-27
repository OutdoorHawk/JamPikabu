using Code.Gameplay.Features.CollidingView.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.CollidingView
{
    public sealed class CollidingViewFeature : Feature
    {
        public CollidingViewFeature(ISystemFactory systems)
        {
            Add(systems.Create<EntityCollisionSystem>());
            Add(systems.Create<EntityTriggeringSystem>());
            
            Add(systems.Create<CleanupCollidedSystem>());
            Add(systems.Create<CleanupTriggeredSystem>());
        }
    }
}