using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class SetLootReadyToApplyOnRoundOverSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _collectedNotAppliedLoot;
        private readonly IGroup<GameEntity> _movingHook;

        public SetLootReadyToApplyOnRoundOverSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundOver
                ));

            _movingHook = context.GetGroup(
                GameMatcher.AllOf(
                        GameMatcher.GrapplingHook)
                    .AnyOf(
                    GameMatcher.AscentRequested,
                    GameMatcher.Descending));

            _collectedNotAppliedLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected
                )
                    .NoneOf(
                        GameMatcher.Applied));
            
            
        }

        public void Execute()
        {
            foreach (var _ in _roundController)
            foreach (var loot in _collectedNotAppliedLoot)
            {
                if (CheckHookIsStillMoving())
                    continue;
                
                loot.isReadyToApply = true;
            }
        }

        private bool CheckHookIsStillMoving()
        {
            return _movingHook.GetEntities().Length != 0;
        }
    }
}