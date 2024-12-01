using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class DestroyAppliedLootSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _loot;

        public DestroyAppliedLootSystem(GameContext context)
        {
            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.Applied
                ));
        }

        public void Cleanup()
        {
            foreach (var loot in _loot)
            {
                loot.isDestructed = true;
            }
        }
    }
}