using Code.Infrastructure.Systems;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessRoundOverWhenTimerDoneSystem : BufferedExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _busyHook;

        protected override int BufferCapacity => 2;

        public ProcessRoundOverWhenTimerDoneSystem(GameContext context)
        {
            _entities = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.CooldownUp
                ));
            
            _busyHook = context.GetGroup(
                GameMatcher.AllOf(
                        GameMatcher.GrapplingHook)
                    .AnyOf(
                        GameMatcher.AscentRequested,
                        GameMatcher.Descending,
                        GameMatcher.CollectingLoot));
        }

        public override void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                if (CheckHookIsStillBusy())
                    continue;

                entity.isCooldownUp = false;
                entity.isRoundOver = true;
            }
        }
        
        private bool CheckHookIsStillBusy()
        {
            return _busyHook.GetEntities().Length != 0;
        }
        
    }
}