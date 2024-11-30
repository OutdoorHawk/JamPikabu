using Code.Gameplay.Features.GrapplingHook.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.GrapplingHook
{
    public sealed class GrapplingHookPhysicsFeature : Feature
    {
        public GrapplingHookPhysicsFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitGrapplingHookSystem>());
            
            Add(systems.Create<SetHookXMovementDirectionByInputSystem>());
        }
    }
}