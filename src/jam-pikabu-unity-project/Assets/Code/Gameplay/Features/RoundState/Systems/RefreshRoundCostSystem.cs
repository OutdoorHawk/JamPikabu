using Code.Gameplay.Features.Currency.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class RefreshRoundCostSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _storages;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;

        public RefreshRoundCostSystem(GameContext context, IGameplayCurrencyService gameplayCurrencyService)
        {
            _gameplayCurrencyService = gameplayCurrencyService;
            _storages = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.DayCost
                ));
        }

        public void Execute()
        {
            foreach (var entity in _storages)
            {
                _gameplayCurrencyService.UpdateCurrentTurnCostAmount(entity.DayCost);
            }
        }
    }
}