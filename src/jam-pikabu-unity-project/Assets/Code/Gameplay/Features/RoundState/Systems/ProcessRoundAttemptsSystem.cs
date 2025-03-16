using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundAttemptsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessRoundAttemptsSystem(GameContext context)
        {
            _entities = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess,
                    GameMatcher.HookAttemptsLeft
                ));
        }

        public void Execute()
        {
            foreach (var controllers in _entities.GetEntities(_buffer))
            {
                if (controllers.HookAttemptsLeft > 0)
                    continue;

                controllers.isRoundInProcess = false;
            }
        }
    }
}