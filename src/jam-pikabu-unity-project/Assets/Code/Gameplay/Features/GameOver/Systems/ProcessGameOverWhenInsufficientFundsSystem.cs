using Code.Gameplay.Features.GameOver.Service;
using Entitas;

namespace Code.Gameplay.Features.GameOver.Systems
{
    public class ProcessGameOverWhenInsufficientFundsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _lootApplier;
        private readonly IGroup<GameEntity> _gold;
        private readonly IGameOverService _gameOverService;

        public ProcessGameOverWhenInsufficientFundsSystem(GameContext context, IGameOverService gameOverService)
        {
            _gameOverService = gameOverService;
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundOver
                ));

            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));

            _gold = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.CurrencyStorage,
                    GameMatcher.Gold
                ));
        }

        public void Execute()
        {
            foreach (var controller in _roundController)
            foreach (var gold in _gold)
            {
                if (LootIsApplying())
                    continue;

                if (gold.Gold >= controller.RoundCost)
                    continue;

                controller.isGameOver = true;
                controller.isDestructed = true;
                _gameOverService.GameOver();
            }
        }

        private bool LootIsApplying()
        {
            return _lootApplier.GetEntities().Length > 0;
        }
    }
}