using System;
using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.Windows.Service;
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
        private readonly ICurrencyFactory _currencyFactory;
        private readonly IWindowService _windowService;

        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _roundStateController;
        private readonly IGroup<GameEntity> _storages;

        private readonly CancellationTokenSource _tearDownSource = new();

        public ProcessNextOrderRequest(GameContext context, IRoundStateService roundStateService, IOrdersService ordersService,
            IGameOverService gameOverService, ICurrencyFactory currencyFactory, IWindowService windowService)
        {
            _roundStateService = roundStateService;
            _ordersService = ordersService;
            _gameOverService = gameOverService;
            _currencyFactory = currencyFactory;
            _windowService = windowService;

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

                if (_ordersService.CheckOrdersCompleted())
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
            if (_roundStateService.CheckAllDaysComplete())
            {
                _roundStateService.DayComplete();
                _gameOverService.GameWin();
                return;
            }

            bool gameOver = storage.Gold < state.DayCost;

            CreateGameEntity
                .Empty()
                .With(x => x.isAddCurrencyRequest = true)
                .AddGold(-state.DayCost)
                ;

            if (_windowService.TryGetWindow(out PlayerHUDWindow hudWindow))
            {
                var source = new UniTaskCompletionSource();

                var currencyHolder = hudWindow.GetComponentInChildren<CurrencyHolder>();
                var parameters = new CurrencyAnimationParameters
                {
                    Type = CurrencyTypeId.Gold,
                    TextPrefix = "-",
                    Count = state.DayCost,
                    StartPosition = currencyHolder.PlayerCurrentGold.CurrencyIcon.transform.position,
                    EndPosition = currencyHolder.PlayerTurnCostGold.CurrencyIcon.transform.position,
                    StartReplenishCallback = () =>
                    {
                        source.TrySetResult();

                        float costSetup = MathF.Max(state.DayCost - storage.Gold, 0);
                        currencyHolder.PlayerTurnCostGold.SetupPrice((int)costSetup, CurrencyTypeId.Gold, true);
                    }
                };

                _currencyFactory.PlayCurrencyAnimation(parameters);

                await source.Task;
            }

            await DelaySeconds(0.75f, _tearDownSource.Token);

            if (gameOver)
                _gameOverService.GameOver();
            else
            {
                _roundStateService.DayComplete();
                _roundStateService.LoadNextDay();
            }
        }

        public void TearDown()
        {
            _tearDownSource.Cancel();
        }
    }
}