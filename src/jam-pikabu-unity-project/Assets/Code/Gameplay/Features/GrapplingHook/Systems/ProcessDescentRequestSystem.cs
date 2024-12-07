using System.Collections.Generic;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class ProcessDescentRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(2);
        private readonly ISoundService _soundService;

        public ProcessDescentRequestSystem(GameContext context, ISoundService soundService)
        {
            _soundService = soundService;
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
                entity.GrapplingHookBehaviour.StartDescending();
                entity.isDescending = true;
            }
        }
    }
}