using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Windows;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Entitas;
using Zenject;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class SkipOrderCheat : BaseCheat, ICheatActionBasic
    {
        private IGameStateService _gameStateService;
        public string CheatLabel => "Пропустить заказ";
        public OrderType Order => OrderType.First;

        [Inject]
        private void Construct(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        public void Execute()
        {
            IGroup<GameEntity> order = _gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.Order));
            
            foreach (var entity in order)
            {
                entity.isResultProcessed = true;
                entity.isComplete = true;
            }
            
            _gameStateService.AskToSwitchState(GameStateTypeId.RoundCompletion);

            _windowService.Close(WindowTypeId.OrderWindow);
        }
    }
}