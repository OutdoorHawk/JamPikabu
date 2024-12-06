using Entitas;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class ContinuousSpawnLootSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public ContinuousSpawnLootSystem(GameContext context)
        {
            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootSpawner,
                    GameMatcher.ContinuousSpawn
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities)
            {
            }
        }
    }
}