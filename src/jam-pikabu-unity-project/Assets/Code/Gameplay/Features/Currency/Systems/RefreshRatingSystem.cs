using Code.Gameplay.Features.Currency.Service;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class RefreshRatingSystem : IExecuteSystem
    {
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IGroup<GameEntity> _plusStorages;
        private readonly IGroup<GameEntity> _minusStorages;

        public RefreshRatingSystem(GameContext context, IGameplayCurrencyService gameplayCurrencyService)
        {
            _gameplayCurrencyService = gameplayCurrencyService;

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
            foreach (var entity in _plusStorages)
            {
                _gameplayCurrencyService.UpdateCurrencyAmount(entity.Plus, CurrencyTypeId.Plus);
            }

            foreach (var entity in _minusStorages)
            {
                _gameplayCurrencyService.UpdateCurrencyAmount(entity.Minus, CurrencyTypeId.Minus);
            }
        }
    }
}