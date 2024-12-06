using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class BlockRoundStartAvailableWhenInsufficientFundsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _gold;

        public BlockRoundStartAvailableWhenInsufficientFundsSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundOver,
                    GameMatcher.DayCost
                ));
            
            _gold = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.CurrencyStorage,
                    GameMatcher.Gold
                ));
        }

        public void Execute()
        {
            foreach (var round in _roundController)
            foreach (var gold in _gold)
            {
                if (gold.Gold < round.DayCost) 
                    round.isRoundStartAvailable = false;
            }
        }
    }
}