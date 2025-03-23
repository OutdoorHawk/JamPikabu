using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundStart.Systems
{
    public class ProcessRoundStartTimerRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessRoundStartTimerRequestSystem(GameContext context)
        {
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
            }
        }
    }
}