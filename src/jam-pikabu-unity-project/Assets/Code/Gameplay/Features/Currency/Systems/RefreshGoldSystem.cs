using Code.Gameplay.Features.Currency.Service;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class RefreshGoldSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _storages;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;

        public RefreshGoldSystem(GameContext context, IGameplayCurrencyService gameplayCurrencyService)
        {
            _gameplayCurrencyService = gameplayCurrencyService;

            _storages = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Gold,
                    GameMatcher.CurrencyStorage
                ));
        }

        public void Execute()
        {
            foreach (var entity in _storages)
            {
                _gameplayCurrencyService.UpdateCurrencyAmount(entity.Gold, entity.Withdraw, CurrencyTypeId.Gold);
            }
        }
    }
}