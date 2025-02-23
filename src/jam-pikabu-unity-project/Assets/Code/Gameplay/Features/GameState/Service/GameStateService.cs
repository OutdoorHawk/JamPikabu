using System;
using Code.Gameplay.Features.GameState.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Days.Service;

namespace Code.Gameplay.Features.GameState.Service
{
    public class GameStateService : IGameStateService
    {
        private readonly IGameStateFactory _gameStateFactory;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IOrdersService _ordersService;
        private readonly IGameplayLootService _gameplayLootService;
        private readonly IDaysService _daysService;

        public GameStateTypeId CurrentState { get; private set; }

        public event Action OnStateSwitched;

        public GameStateService
        (
            IGameStateFactory gameStateFactory,
            IGameStateMachine gameStateMachine,
            IOrdersService ordersService,
            IGameplayLootService gameplayLootService,
            IDaysService daysService
        )
        {
            _gameStateFactory = gameStateFactory;
            _gameStateMachine = gameStateMachine;
            _ordersService = ordersService;
            _gameplayLootService = gameplayLootService;
            _daysService = daysService;
        }

        public void AskToSwitchState(GameStateTypeId newState)
        {
            _gameStateFactory.CreateSwitchGameStateRequest(newState);
        }

        public void CompleteStateSwitch(GameStateTypeId newState)
        {
            switch (newState)
            {
                case GameStateTypeId.Unknown:
                    break;
                case GameStateTypeId.BeginDay:
                    EnterBeginDay();
                    break;
                case GameStateTypeId.RoundPreparation:
                    EnterRoundPreparation();
                    break;
                case GameStateTypeId.RoundLoop:
                    break;
                case GameStateTypeId.RoundCompletion:
                    EnterRoundCompletion();
                    break;
                case GameStateTypeId.EndDay:
                    EnterEndDay();
                    break;
            }

            NotifyStateSwitched(newState);
        }

        private void NotifyStateSwitched(GameStateTypeId newState)
        {
            CurrentState = newState;
            OnStateSwitched?.Invoke();
        }

        private void EnterBeginDay()
        {
            switch (_daysService.BonusLevelType)
            {
                case BonusLevelType.GoldenCoins:
                {
                    _daysService.BeginDay();
                    _gameplayLootService.CreateLootSpawner();
                    break;
                }
                default:
                {
                    _daysService.BeginDay();
                    _ordersService.InitDayBegin();
                    _gameplayLootService.CreateLootSpawner();
                    break;
                }
            }
        }

        private void EnterRoundPreparation()
        {
            switch (_daysService.BonusLevelType)
            {
                case BonusLevelType.GoldenCoins:
                {
                    _daysService.EnterRoundPreparation();
                    _gameplayLootService.ClearCollectedLoot();
                    break;
                }
                default:
                {
                    _ordersService.CreateOrder();
                    _gameplayLootService.CreateLootSpawner();
                    _daysService.EnterRoundPreparation();
                    _gameplayLootService.ClearCollectedLoot();
                    break;
                }
            }
        }

        private void EnterRoundCompletion()
        {
            switch (_daysService.BonusLevelType)
            {
                case BonusLevelType.GoldenCoins:
                    AskToSwitchState(GameStateTypeId.EndDay);
                    break;
                default:
                    _gameplayLootService.CreateLootConsumer();
                    break;
            }
        }

        private void EnterEndDay()
        {
            _gameplayLootService.DayEnd();
        }
    }
}