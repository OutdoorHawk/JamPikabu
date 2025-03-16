using Code.Infrastructure.Systems;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class MoveToRoundCompleteStateWhenHookAreNotBusySystem : BufferedExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundStateController;

        protected override int BufferCapacity => 2;

        public MoveToRoundCompleteStateWhenHookAreNotBusySystem(GameContext context)
        {
            _roundStateController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.CooldownUp
                ));
        }

        public override void Execute()
        {
            foreach (var entity in _roundStateController.GetEntities(_buffer))
            {
                entity.isCooldownUp = false;
                entity.isRoundOver = true;
                entity.isRoundComplete = true;
            }
        }
    }
}