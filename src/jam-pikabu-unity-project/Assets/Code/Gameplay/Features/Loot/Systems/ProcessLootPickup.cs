using Code.Gameplay.Features.Loot.Service;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ProcessLootPickup : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly ILootUIService _lootUIService;

        public ProcessLootPickup(GameContext context, ILootUIService lootUIService)
        {
            _lootUIService = lootUIService;

            _entities = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Loot,
                    GameMatcher.CollectLootRequest
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