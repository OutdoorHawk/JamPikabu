using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Cysharp.Threading.Tasks;
using Entitas;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessNextOrderRequest : IExecuteSystem, ITearDownSystem
    {
        private readonly IRoundStateService _roundStateService;
        private readonly IOrdersService _ordersService;
        private readonly IGameOverService _gameOverService;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _roundStateController;
        private readonly IGroup<GameEntity> _storages;

        private CancellationTokenSource _tearDownSource = new();

        public ProcessNextOrderRequest(GameContext context, IRoundStateService roundStateService, IOrdersService ordersService,
            IGameOverService gameOverService)
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

            _storages = context.GetGroup(GameMatcher
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
                
                if (_ordersService.OrdersCompleted())
                {
                    TryPayQuota(storage, state).Forget();
                }
                else
                {
                    _ordersService.GoToNextOrder();
                }
            }
        }

        private async UniTaskVoid TryPayQuota(GameEntity storage, GameEntity state)
        {
            bool gameOver = storage.Gold < state.DayCost;

            CreateGameEntity
                .Empty()
                .With(x => x.isAddCurrencyRequest = true)
                .AddGold(-state.DayCost)
                ;

            await DelaySeconds(1, _tearDownSource.Token);

            if (gameOver)
                _gameOverService.GameOver();
            else
                _roundStateService.DayComplete();
        }

        public void TearDown()
        {
            _tearDownSource.Cancel();
        }
    }
}