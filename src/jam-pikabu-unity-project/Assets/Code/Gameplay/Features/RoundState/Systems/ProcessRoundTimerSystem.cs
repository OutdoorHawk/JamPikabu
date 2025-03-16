using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundTimerSystem : IExecuteSystem
    {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _roundStateView;
        
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessRoundTimerSystem(GameContext context, ITimeService time)
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
            foreach (var controllers in _entities.GetEntities(_buffer))
            {
                if (controllers.RoundTimeLeft > 0)
                {
                    float newTime = controllers.RoundTimeLeft - _time.DeltaTime;
                    controllers.ReplaceRoundTimeLeft(newTime);
                    continue;
                }
                
                controllers.isRoundEndRequest = true;
            }
        }
    }
}