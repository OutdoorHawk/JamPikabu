using Code.Infrastructure.Systems;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class ProcessAscentRequestSystem : BufferedExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        protected override int BufferCapacity => 2;

        public ProcessAscentRequestSystem(GameContext context)
        {
            _entities = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.GrapplingHook,
                    GameMatcher.AscentRequested,
                    GameMatcher.AscentAvailable
                ));
        }

        public override void Execute()
        {
            foreach (var hook in _entities.GetEntities(_buffer))
            {
                hook.isAscending = true;
                hook.isAscentRequested = false;
            }
        }
    }
}