using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.GameState.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Code.Infrastructure.States.StateMachine;

namespace Code.Gameplay.Features.GameState.Service
{
    public class GameStateService : IGameStateService
    {
        private readonly IGameStateFactory _gameStateFactory;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IOrdersService _ordersService;
        private readonly ILootService _lootService;
        private readonly IRoundStateService _roundStateService;

        public GameStateService
        (
            IGameStateFactory gameStateFactory,
            IGameStateMachine gameStateMachine,
            IOrdersService ordersService,
            ILootService lootService,
            IRoundStateService roundStateService
        )
        {
            _gameStateFactory = gameStateFactory;
            _gameStateMachine = gameStateMachine;
            _ordersService = ordersService;
            _lootService = lootService;
            _roundStateService = roundStateService;
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
                    _roundStateService.BeginDay();
                    _ordersService.InitDayBegin();
                    _lootService.CreateLootSpawner();
                    SaveStash();
                    break;
                case GameStateTypeId.RoundPreparation:
                    _ordersService.CreateOrder();
                    _roundStateService.EnterRoundPreparation();
                    _lootService.ClearCollectedLoot();
                    break;
                case GameStateTypeId.RoundLoop:
                    break;
                case GameStateTypeId.RoundCompletion:
                    _lootService.CreateLootConsumer();
                    break;
                case GameStateTypeId.EndDay:
                    break;
            }
        }
        
        private void SaveStash()
        {
            foreach (var storage in Contexts.sharedInstance.game.GetGroup(GameMatcher.AllOf(GameMatcher.CurrencyStorage, GameMatcher.Gold)))
                GameplayCurrencyService.CurrencyCache[CurrencyTypeId.Gold] = storage.Gold;
            foreach (var storage in Contexts.sharedInstance.game.GetGroup(GameMatcher.AllOf(GameMatcher.CurrencyStorage, GameMatcher.Plus)))
                GameplayCurrencyService.CurrencyCache[CurrencyTypeId.Plus] = storage.Plus;
            foreach (var storage in Contexts.sharedInstance.game.GetGroup(GameMatcher.AllOf(GameMatcher.CurrencyStorage, GameMatcher.Minus)))
                GameplayCurrencyService.CurrencyCache[CurrencyTypeId.Minus] = storage.Minus;
        }
    }
}