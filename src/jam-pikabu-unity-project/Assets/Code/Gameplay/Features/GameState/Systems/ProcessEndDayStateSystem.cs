using System;
using System.Collections.Generic;
using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessEndDayStateSystem : IExecuteSystem, ITearDownSystem
    {
        private readonly IGameStateService _gameStateService;
        private readonly IRoundStateService _roundService;
        private readonly IOrdersService _ordersService;
        private readonly ILootService _lootService;
        private readonly IGameOverService _gameOverService;
        private readonly IWindowService _windowService;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly IGameStateMachine _stateMachine;

        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _goldStorage;
        private readonly List<GameEntity> _buffer = new();
        private readonly CancellationTokenSource _tearDownSource = new();
        private readonly IGroup<GameEntity> _roundState;
        private readonly IGroup<MetaEntity> _days;

        public ProcessEndDayStateSystem
        (
            GameContext context,
            IGameStateService gameStateService,
            IRoundStateService roundService,
            IOrdersService ordersService,
            ILootService lootService,
            IGameOverService gameOverService,
            IWindowService windowService,
            ICurrencyFactory currencyFactory,
            IGameStateMachine stateMachine,
            MetaContext metaContext
        )
        {
            _gameStateService = gameStateService;
            _roundService = roundService;
            _ordersService = ordersService;
            _lootService = lootService;
            _gameOverService = gameOverService;
            _windowService = windowService;
            _currencyFactory = currencyFactory;
            _stateMachine = stateMachine;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.EndDay,
                    GameMatcher.StateProcessingAvailable
                ));

            _goldStorage = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Gold,
                    GameMatcher.CurrencyStorage
                ));
            
            _roundState = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.RoundStateController
                ));
            
            _days = metaContext.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Day
                ));
        }

        public void Execute()
        {
            foreach (var gameState in _entities.GetEntities(_buffer))
            foreach (var storage in _goldStorage)
            foreach (var roundState in _roundState)
            {
                gameState.isStateProcessingAvailable = false;

                CheckGameWinOrLoseConditions(storage, gameState, roundState);
                _lootService.ClearCollectedLoot();
            }
        }

        private void CheckGameWinOrLoseConditions(GameEntity storage, GameEntity gameState, GameEntity roundState)
        {
            ProcessRegularDayEnd(storage, gameState);
            TryPayQuota(storage, gameState, roundState).Forget();
        }

        private void ProcessRegularDayEnd(GameEntity storage, GameEntity gameState)
        {
            DayData dayData = _roundService.GetDayData();
            gameState.isPassesPlayCost = dayData.PlayCost <= storage.Gold;
        }

        private async UniTaskVoid TryPayQuota(GameEntity storage, GameEntity state, GameEntity roundState)
        {
            CreateGameEntity
                .Empty()
                .With(x => x.isAddCurrencyRequest = true)
                .AddGold(-roundState.DayCost)
                ;
            
            await PayForProductsAnimation(storage, roundState);

            await DelaySeconds(0.75f, _tearDownSource.Token);

            if (state.isPassesPlayCost == false)
            {
                state.isGameOver = true;
                _gameOverService.GameOver();
                return;
            }

            foreach (var day in _days)
            {
                int newValue = Mathf.Min(_roundService.MaxDays, day.Day + 1);
                day.ReplaceDay(newValue);
            }
            
            _stateMachine.Enter<GameOverState>();
            await UniTask.Yield();
            await UniTask.Yield();
            _stateMachine.Enter<LoadLevelSimpleState, LoadLevelPayloadParameters>(new LoadLevelPayloadParameters());
        }

        private async UniTask PayForProductsAnimation(GameEntity storage, GameEntity roundState)
        {
            if (_windowService.TryGetWindow(out PlayerHUDWindow hudWindow))
            {
                var source = new UniTaskCompletionSource();

                var currencyHolder = hudWindow.GetComponentInChildren<CurrencyHolder>();
                var parameters = new CurrencyAnimationParameters
                {
                    Type = CurrencyTypeId.Gold,
                    TextPrefix = "-",
                    Count = roundState.DayCost,
                    BeginAnimationSound = SoundTypeId.PurchasedQuota,
                    StartPosition = currencyHolder.PlayerCurrentGold.CurrencyIcon.transform.position,
                    EndPosition = currencyHolder.PlayerTurnCostGold.CurrencyIcon.transform.position,
                    StartReplenishCallback = () =>
                    {
                        source.TrySetResult();

                        
                        currencyHolder.PlayerTurnCostGold.SetupPrice(0, CurrencyTypeId.Gold, true);
                    }
                };

                _currencyFactory.PlayCurrencyAnimation(parameters);

                await source.Task;
            }
        }

        public void TearDown()
        {
            _tearDownSource?.Cancel();
        }
    }
}