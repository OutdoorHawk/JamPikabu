using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundSystem : IExecuteSystem
    {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessRoundSystem(GameContext context, ITimeService time)
        {
            _time = time;
            _entities = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess,
                    GameMatcher.RoundTimeLeft
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                if (entity.RoundTimeLeft > 0)
                {
                    entity.ReplaceRoundTimeLeft(entity.RoundTimeLeft - _time.DeltaTime);
                    continue;
                }

                entity.isRoundInProcess = false;
                entity.isRoundOver = true;
            }
        }
    }
}