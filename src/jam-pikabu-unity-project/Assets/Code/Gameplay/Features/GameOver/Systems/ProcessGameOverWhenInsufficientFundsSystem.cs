using Code.Gameplay.Features.GameOver.Service;
using Entitas;

namespace Code.Gameplay.Features.GameOver.Systems
{
    public class ProcessGameOverWhenInsufficientFundsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _loot;
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

            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected
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
            return _loot.GetEntities().Length > 0;
        }
    }
}