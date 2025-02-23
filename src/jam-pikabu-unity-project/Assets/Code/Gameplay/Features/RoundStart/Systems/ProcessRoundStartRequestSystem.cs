using System.Collections.Generic;
using Code.Infrastructure.States.StateMachine;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundStartRequestSystem : IExecuteSystem
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessRoundStartRequestSystem(GameContext context, IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _entities = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundStartRequest,
                    GameMatcher.RoundStartAvailable
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                entity.isRoundOver = false;
                entity.isRoundInProcess = true;
                entity.isRoundComplete = false;
                entity.AddRoundTimeLeft(entity.RoundDuration);
            }
        }
    }
}