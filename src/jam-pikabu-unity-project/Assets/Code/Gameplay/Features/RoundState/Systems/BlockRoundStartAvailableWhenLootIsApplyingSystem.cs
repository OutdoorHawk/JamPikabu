using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class BlockRoundStartAvailableWhenLootIsApplyingSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _lootApplier;

        public BlockRoundStartAvailableWhenLootIsApplyingSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController
                ));

            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
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
            return _lootApplier.GetEntities().Length > 0;
        }
    }
}