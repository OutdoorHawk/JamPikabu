using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundStart.Systems
{
    public class ResetRoundTimerOnEnterRoundPreparationSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new(2);

        public ResetRoundTimerOnEnterRoundPreparationSystem(GameContext context)
        {
            _entities = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController
                ));

            _requests = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.SwitchGameStateRequest,
                    GameMatcher.RoundPreparation
                ));
        }

        public void Execute()
        {
            foreach (var _ in _requests)
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                if (entity.hasRoundDuration)
                    entity.ReplaceRoundTimeLeft(entity.RoundDuration);

                if (entity.hasHookAttemptsMax)
                    entity.ReplaceHookAttemptsLeft(entity.HookAttemptsMax);
            }
        }
    }
}