using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class InitLootSystem : IInitializeSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public InitLootSystem(GameContext context)
        {
           
        }

        public void Initialize()
        {
            foreach (var entity in _entities)
            {
            }
        }
    }
}