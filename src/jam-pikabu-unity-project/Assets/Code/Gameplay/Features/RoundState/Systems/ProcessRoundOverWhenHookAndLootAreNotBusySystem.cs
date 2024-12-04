using Code.Infrastructure.Systems;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundOverWhenHookAndLootAreNotBusySystem : BufferedExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundStateController;
        private readonly IGroup<GameEntity> _busyHook;
        private readonly IGroup<GameEntity> _lootAppliers;
        private readonly IGroup<GameEntity> _busyLoot;

        protected override int BufferCapacity => 2;

        public ProcessRoundOverWhenHookAndLootAreNotBusySystem(GameContext context)
        {
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