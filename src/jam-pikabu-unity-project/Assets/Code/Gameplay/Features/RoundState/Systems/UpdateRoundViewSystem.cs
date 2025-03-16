using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class UpdateRoundViewSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundStateControllers1;
        private readonly IGroup<GameEntity> _roundStateControllers2;
        private readonly IGroup<GameEntity> _roundStateView;

        private readonly List<GameEntity> _buffer = new(2);

        public UpdateRoundViewSystem(GameContext context)
        {
            _roundStateControllers1 = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.HookAttemptsLeft
                ));

            _roundStateControllers2 = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundTimeLeft
                ));

            _roundStateView = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateViewBehaviour
                ));
        }

        public void Execute()
        {
            foreach (var controllers in _roundStateControllers1.GetEntities(_buffer))
            {
                UpdateTimerView(controllers.HookAttemptsLeft);
            }

            foreach (var controllers in _roundStateControllers2.GetEntities(_buffer))
            {
                UpdateTimerView((int)controllers.RoundTimeLeft);
            }
        }

        private void UpdateTimerView(int time)
        {
            foreach (var view in _roundStateView)
            {
                view.RoundStateViewBehaviour.UpdateRoundStateCounter(time);
            }
        }
    }
}