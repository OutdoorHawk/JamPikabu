using Code.Gameplay.Features.RoundState.Service;
using Code.Infrastructure.Systems;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class MoveToRoundCompleteStateWhenHookAreNotBusySystem : BufferedExecuteSystem
    {
        private readonly IRoundStateService _roundStateService;
        private readonly IGroup<GameEntity> _roundStateController;
        private readonly IGroup<GameEntity> _busyHook;
        private readonly IGroup<GameEntity> _busyLoot;

        protected override int BufferCapacity => 2;

        public MoveToRoundCompleteStateWhenHookAreNotBusySystem(GameContext context, IRoundStateService roundStateService)
        {
            _roundStateService = roundStateService;
            _busyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Busy
                ));

            _busyHook = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.GrapplingHook,
                    GameMatcher.Busy));

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
                if (CheckHookIsStillBusy())
                    continue;

                if (CheckLootIsStillBusy())
                    continue;

                entity.isCooldownUp = false;
                entity.isRoundOver = true;
                entity.isRoundComplete = true;
                _roundStateService.RoundComplete();
            }
        }

        private bool CheckHookIsStillBusy()
        {
            return _busyHook.GetEntities().Length != 0;
        }

        private bool CheckLootIsStillBusy()
        {
            return _busyLoot.GetEntities().Length != 0;
        }
    }
}