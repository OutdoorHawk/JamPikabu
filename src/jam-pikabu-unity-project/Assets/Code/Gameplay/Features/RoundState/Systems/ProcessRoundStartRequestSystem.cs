using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundStartRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessRoundStartRequestSystem(GameContext context)
        {
            _entities = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundStartRequest,
                    GameMatcher.RoundStartAvailable
                ).NoneOf(
                    GameMatcher.RoundInProcess));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                entity.isRoundInProcess = true;
                entity.ReplaceRoundTimeLeft(entity.RoundDuration);
            }
        }
    }
}