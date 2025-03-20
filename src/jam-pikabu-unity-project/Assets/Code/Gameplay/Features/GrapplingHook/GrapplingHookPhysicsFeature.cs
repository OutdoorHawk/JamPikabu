using Code.Gameplay.Features.CharacterStats;
using Code.Gameplay.Features.GrapplingHook.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.GrapplingHook
{
    public sealed class GrapplingHookPhysicsFeature : Feature
    {
        public GrapplingHookPhysicsFeature(ISystemFactory systems)
        {
            Add(systems.Create<SetHookXMovementDirectionByInputSystem>());
            Add(systems.Create<SetHookDescentRequestByInputSystem>());

            Add(systems.Create<BlockGrapplingHookMovementWhenRoundNotProcessingSystem>());
            Add(systems.Create<BlockGrapplingHookXMovementWhenDescendingSystem>());
            Add(systems.Create<BlockGrapplingHookXMovementWhenClosingClawsSystem>());
            Add(systems.Create<BlockGrapplingHookMovementWhenAscendingSystem>());
            Add(systems.Create<BlockGrapplingHookMovementWhenCollectingLootSystem>());
            Add(systems.Create<BlockGrapplingHookMovementWhenAnyOtherWindowOpenSystem>());

            Add(systems.Create<MoveGrapplingHookByXAxisSystem>());

            Add(systems.Create<ProcessDescentRequestSystem>());
            Add(systems.Create<DescentGrapplingHookSystem>());
            Add(systems.Create<MarkLootAscendingInsideHookSystem>());
            
            Add(systems.Create<RemoveHookAttemptOnDescentSystem>());

            Add(systems.Create<ProcessAscentRequestSystem>());
            Add(systems.Create<AscentGrapplingHookSystem>());
            Add(systems.Create<UpdateHookStatChangesOnAscendCompleteSystem>());

            Add(systems.Create<GrapplingHookCollectLootSystem>());

            Add(systems.Create<UpdateGrapplingHookBusyStateSystem>());

            Add(systems.Create<GrapplingHookVisualsSystem>());
            Add(systems.Create<ApplyGrapplingHookScaleSystem>());
            
            Add(systems.Create<ResetGrapplingHookMovementSystem>());
        }
    }

    public sealed class GrapplingHookFeature : Feature
    {
        public GrapplingHookFeature(ISystemFactory systems)
        {
            Add(systems.Create<CleanupLootInsideHookSystem>());
        }
    }
}