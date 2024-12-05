using Code.Gameplay.Features.Currency.Service;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class ProcessAddGoldRequestSystem : IExecuteSystem
    {
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IGroup<GameEntity> _storages;
        private readonly IGroup<GameEntity> _requests;

        public ProcessAddGoldRequestSystem(GameContext context, IGameplayCurrencyService gameplayCurrencyService)
        {
            _gameplayCurrencyService = gameplayCurrencyService;
            _requests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Gold,
                    GameMatcher.AddGoldRequest
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
                    _gameplayCurrencyService.AddWithdraw(request.Withdraw);
            }
        }
    }
}