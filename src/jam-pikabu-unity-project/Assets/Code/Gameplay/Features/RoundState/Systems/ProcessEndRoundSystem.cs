using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessEndRoundSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundStateControllers;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessEndRoundSystem(GameContext context)
        {
            _roundStateControllers = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess,
                    GameMatcher.RoundEndRequest
                ));
        }

        public void Execute()
        {
            foreach (var controllers in _roundStateControllers.GetEntities(_buffer))
            {
                controllers.isRoundEndRequest = false;
                controllers.isRoundInProcess = false;
                controllers.isCooldownUp = true;
            }
        }
        
    }
}