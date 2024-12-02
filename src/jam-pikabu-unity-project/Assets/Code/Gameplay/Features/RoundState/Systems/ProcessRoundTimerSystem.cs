using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundTimerSystem : IExecuteSystem
    {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);
        private readonly IGroup<GameEntity> _roundStateView;

        public ProcessRoundTimerSystem(GameContext context, ITimeService time)
        {
            _time = time;
            
            _entities = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess,
                    GameMatcher.RoundTimeLeft
                ));
            
            _roundStateView = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateViewBehaviour
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            foreach (var view in _roundStateView)
            {
                if (entity.RoundTimeLeft > 0)
                {
                    float newTime = entity.RoundTimeLeft - _time.DeltaTime;
                    entity.ReplaceRoundTimeLeft(newTime);
                    view.RoundStateViewBehaviour.UpdateTimer(newTime);
                    continue;
                }

                entity.isCooldownUp = true;
                entity.RemoveRoundTimeLeft();
               
                view.RoundStateViewBehaviour.UpdateTimer(0);
            }
        }
    }
}