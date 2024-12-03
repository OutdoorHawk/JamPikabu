using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class BlockRoundStartAvailableWhenLootIsApplyingSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _loot;

        public BlockRoundStartAvailableWhenLootIsApplyingSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController
                ));

            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Collected,
                    GameMatcher.Loot
                ));
        }

        public void Execute()
        {
            foreach (var controller in _roundController)
            {
                if (LootIsApplying())
                {
                    controller.isRoundStartAvailable = false;
                }
            }
        }

        private bool LootIsApplying()
        {
            return _loot.GetEntities().Length > 0;
        }
    }
}