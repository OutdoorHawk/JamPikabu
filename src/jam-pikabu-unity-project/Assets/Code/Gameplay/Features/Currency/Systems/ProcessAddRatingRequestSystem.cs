using Code.Gameplay.Features.Currency.Service;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class ProcessAddRatingRequestSystem : IExecuteSystem
    {
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IGroup<GameEntity> _plusStorages;
        private readonly IGroup<GameEntity> _plusRequests;
        private readonly IGroup<GameEntity> _minusRequests;
        private readonly IGroup<GameEntity> _minusStorages;

        public ProcessAddRatingRequestSystem(GameContext context, IGameplayCurrencyService gameplayCurrencyService)
        {
            _gameplayCurrencyService = gameplayCurrencyService;

            _plusRequests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Plus,
                    GameMatcher.AddCurrencyRequest
                ));

            _minusRequests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Minus,
                    GameMatcher.AddCurrencyRequest
                ));

            _plusStorages = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Plus,
                    GameMatcher.CurrencyStorage
                ));

            _minusStorages = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Minus,
                    GameMatcher.CurrencyStorage
                ));
        }

        public void Execute()
        {
            foreach (var request in _plusRequests)
            foreach (var storage in _plusStorages)
            {
                request.isDestructed = true;

                storage.ReplacePlus(storage.Plus + request.Plus);
                
                if (request.hasWithdraw) 
                    storage.ReplaceWithdraw(storage.Withdraw + request.Withdraw);
            }

            foreach (var request in _minusRequests)
            foreach (var storage in _minusStorages)
            {
                request.isDestructed = true;

                storage.ReplaceMinus(storage.Minus + request.Minus);
                
                if (request.hasWithdraw) 
                    storage.ReplaceWithdraw(storage.Withdraw + request.Withdraw);
            }
        }
    }
}