using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class CleanupCollectLootRequestSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _loot;
        private readonly IGroup<GameEntity> _lootApplier;
        private readonly List<GameEntity> _buffer = new(16);

        public CleanupCollectLootRequestSystem(GameContext context)
        {
            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.CollectLootRequest
                ));
        }

        public void Cleanup()
        {
            foreach (GameEntity entity in _loot.GetEntities(_buffer))
            {
                entity.isCollectLootRequest = false;
            }
        }
    }
}