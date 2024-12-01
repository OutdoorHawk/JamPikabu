using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class ProcessDescentRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessDescentRequestSystem(GameContext context)
        {
            _entities = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.GrapplingHook,
                    GameMatcher.DescentRequested,
                    GameMatcher.DescentAvailable
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                entity.isDescending = true;
            }
        }
    }
}