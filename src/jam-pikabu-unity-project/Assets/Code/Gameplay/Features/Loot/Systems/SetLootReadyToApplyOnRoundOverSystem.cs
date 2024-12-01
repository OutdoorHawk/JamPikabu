using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class SetLootReadyToApplyOnRoundOverSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _loot;

        public SetLootReadyToApplyOnRoundOverSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundOver
                ));

            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected
                ));
        }

        public void Execute()
        {
            foreach (var _ in _roundController)
            foreach (var loot in _loot)
            {
                loot.isReadyToApply = true;
            }
        }
    }
}