using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.GameState.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class InitializeGameStateSystem : IInitializeSystem
    {
        private readonly IGameStateService _gameStateService;
        private readonly IGroup<GameEntity> _entities;

        public InitializeGameStateSystem(GameContext context, IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        public void Initialize()
        {
            CreateGameEntity.Empty()
                .With(x => x.isGameState = true);
            
            _gameStateService.AskToSwitchState(GameStateTypeId.BeginDay);
        }
    }
}