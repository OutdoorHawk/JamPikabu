using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessNextOrderRequest : IExecuteSystem
    {
        private readonly IRoundStateService _roundStateService;
        private readonly IOrdersService _ordersService;
        private readonly IGameOverService _gameOverService;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _roundStateController;
        private readonly IGroup<GameEntity> _storages;

        public ProcessNextOrderRequest(GameContext context, IRoundStateService roundStateService, IOrdersService ordersService, IGameOverService gameOverService)
        {
            _roundStateService = roundStateService;
            _ordersService = ordersService;
            _gameOverService = gameOverService;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.NextOrderRequest
                ));

            _roundStateController = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.RoundStateController
                ));
            
            _storages =  context.GetGroup(GameMatcher
                .AllOf(GameMatcher.CurrencyStorage, 
                    GameMatcher.Gold
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities)
            foreach (var state in _roundStateController)
            foreach (var storage in _storages)
            {
                entity.isDestructed = true;

                _ordersService.GoToNextOrder();

                if (_ordersService.OrdersCompleted() == false)
                    continue;

                if (storage.Gold < state.DayCost) 
                    _gameOverService.GameOver();
                else
                    _roundStateService.DayComplete();
            }
        }
    }
}