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
            Add(systems.Create<SetHookDescentByInputSystem>());
            
            Add(systems.Create<BlockGrapplingHookMovementWhenLootSpawningSystem>());
            Add(systems.Create<BlockGrapplingHookXMovementWhenDescendingSystem>());
            Add(systems.Create<BlockGrapplingHookMovementWhenAscendingSystem>());
            Add(systems.Create<BlockGrapplingHookMovementWhenCollectingLootSystem>());
            
            Add(systems.Create<MoveGrapplingHookByXAxisSystem>());
            
            Add(systems.Create<DescentGrapplingHookSystem>());
            Add(systems.Create<AscentGrapplingHookSystem>());
            
            Add(systems.Create<GrapplingHookVisualsSystem>());

            Add(systems.Create<ResetGrapplingHookMovementSystem>());
        }
    }
}