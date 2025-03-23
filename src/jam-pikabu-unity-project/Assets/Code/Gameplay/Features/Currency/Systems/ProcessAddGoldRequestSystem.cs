using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class ProcessAddGoldRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _storages;
        private readonly IGroup<GameEntity> _requests;

        public ProcessAddGoldRequestSystem(GameContext context)
        {
            _requests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Gold,
                    GameMatcher.AddCurrencyRequest
                ));

            _storages = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Gold,
                    GameMatcher.CurrencyStorage
                ));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            foreach (var storage in _storages)
            {
                request.isDestructed = true;

                storage.ReplaceGold(storage.Gold + request.Gold);

                if (request.hasWithdraw)
                    storage.ReplaceWithdraw(storage.Withdraw + request.Withdraw);

                storage.ReplaceEarnedInDay(storage.EarnedInDay + request.Gold);
                
                storage.ReplaceCurrencyAmount(storage.Gold);
            }
        }
    }
}