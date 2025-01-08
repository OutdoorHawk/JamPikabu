using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class CleanupLootInsideHookSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _lootInHook;
        private readonly List<GameEntity> _buffer = new(16);

        public CleanupLootInsideHookSystem(GameContext gameContext)
        {
            _lootInHook = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.InsideHook
                ));
        }
        
        public void Cleanup()
        {
            foreach (GameEntity gameEntity in _lootInHook.GetEntities(_buffer))
            {
                gameEntity.isInsideHook = false;
            }
        }
    }
}