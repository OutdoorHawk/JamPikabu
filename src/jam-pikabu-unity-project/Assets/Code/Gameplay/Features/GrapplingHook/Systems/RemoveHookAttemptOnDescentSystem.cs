using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class RemoveHookAttemptOnDescentSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;
        private readonly IGroup<GameEntity> _roundStateControllers;
        private readonly List<GameEntity> _buffer = new(2);

        public RemoveHookAttemptOnDescentSystem(GameContext context)
        {
            _hooks = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.GrapplingHook,
                    GameMatcher.DescentRequested,
                    GameMatcher.XAxisMovementAvailable,
                    GameMatcher.DescentAvailable)
            );

            _roundStateControllers = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.HookAttemptsMax,
                    GameMatcher.HookAttemptsLeft
                ));
        }

        public void Execute()
        {
            foreach (var roundState in _roundStateControllers)
            foreach (var hook in _hooks.GetEntities(_buffer))
            {
                if (roundState.HookAttemptsLeft <= 0)
                    continue;

                roundState.ReplaceHookAttemptsLeft(roundState.HookAttemptsLeft - 1);
            }
        }
    }
}