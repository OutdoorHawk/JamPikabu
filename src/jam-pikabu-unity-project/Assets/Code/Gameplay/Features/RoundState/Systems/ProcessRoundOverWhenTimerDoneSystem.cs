using Code.Infrastructure.Systems;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundOverWhenTimerDoneSystem : BufferedExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundStateController;
        private readonly IGroup<GameEntity> _busyHook;
        private readonly IGroup<GameEntity> _lootAppliers;

        protected override int BufferCapacity => 2;

        public ProcessRoundOverWhenTimerDoneSystem(GameContext context)
        {
            _roundStateController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.CooldownUp
                ));

            _busyHook = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.GrapplingHook, 
                    GameMatcher.Busy));
        }

        public override void Execute()
        {
            foreach (var entity in _roundStateController.GetEntities(_buffer))
            {
                if (CheckHookIsStillBusy())
                    continue;
                
                entity.isCooldownUp = false;
                entity.isRoundInProcess = false;
                entity.isRoundOver = true;
            }
        }

        private bool CheckHookIsStillBusy()
        {
            return _busyHook.GetEntities().Length != 0;
        }
    }
}