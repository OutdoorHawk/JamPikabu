using Code.Infrastructure.Systems;
using Code.Meta.Features.Days.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class MoveToRoundCompleteStateWhenHookAreNotBusySystem : BufferedExecuteSystem
    {
        private readonly IDaysService _daysService;
        private readonly IGroup<GameEntity> _roundStateController;

        protected override int BufferCapacity => 2;

        public MoveToRoundCompleteStateWhenHookAreNotBusySystem(GameContext context, IDaysService daysService)
        {
            _daysService = daysService;

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
                _daysService.RoundEnd();
            }
        }
    }
}