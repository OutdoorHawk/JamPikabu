using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundAttemptsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);
        private readonly IGroup<GameEntity> _roundStateView;

        public ProcessRoundAttemptsSystem(GameContext context)
        {
            _entities = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess,
                    GameMatcher.HookAttemptsLeft
                ));

            _roundStateView = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateViewBehaviour
                ));
        }

        public void Execute()
        {
            foreach (var controllers in _entities.GetEntities(_buffer))
            {
                if (controllers.HookAttemptsLeft > 0)
                {
                    UpdateTimerView(controllers.HookAttemptsLeft);
                    continue;
                }

                controllers.isCooldownUp = true;
                controllers.isRoundInProcess = false;
                controllers.RemoveHookAttemptsLeft();

                UpdateTimerView(0);
            }
        }

        private void UpdateTimerView(int time)
        {
            foreach (var view in _roundStateView)
            {
                view.RoundStateViewBehaviour.UpdateTimer(time);
            }
        }
    }
}