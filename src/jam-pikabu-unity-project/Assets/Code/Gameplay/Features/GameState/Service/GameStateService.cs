using Code.Gameplay.Features.GameState.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Service;

namespace Code.Gameplay.Features.GameState.Service
{
    public class GameStateService : IGameStateService
    {
        private readonly IGameStateFactory _gameStateFactory;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IOrdersService _ordersService;
        private readonly ILootService _lootService;
        private readonly IDaysService _daysService;

        public GameStateService
        (
            IGameStateFactory gameStateFactory,
            IGameStateMachine gameStateMachine,
            IOrdersService ordersService,
            ILootService lootService,
            IDaysService daysService
        )
        {
            _gameStateFactory = gameStateFactory;
            _gameStateMachine = gameStateMachine;
            _ordersService = ordersService;
            _lootService = lootService;
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
                    break;
            }
        }

        private void EnterBeginDay()
        {
            _daysService.BeginDay();
            _ordersService.InitDayBegin();
            _lootService.CreateLootSpawner();
        }

        private void EnterRoundPreparation()
        {
            _ordersService.CreateOrder();
            _daysService.EnterRoundPreparation();
            _lootService.ClearCollectedLoot();
        }

        private void EnterRoundCompletion()
        {
            _lootService.CreateLootConsumer();
        }
    }
}