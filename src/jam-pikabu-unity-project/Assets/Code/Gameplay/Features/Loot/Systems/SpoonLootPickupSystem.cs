using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class SpoonLootPickupSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _spoons;
        private readonly IGroup<GameEntity> _otherLoot;

        public SpoonLootPickupSystem
        (
            GameContext context
        )
        {
            _spoons = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Spoon,
                    GameMatcher.CollectLootRequest
                ));

            _otherLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.Rigidbody2D
                ).NoneOf(
                    GameMatcher.MarkedForPickup));

            context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess,
                    GameMatcher.RoundTimeLeft
                ));
        }

        public void Execute()
        {
            foreach (var _ in _spoons)
            {
                ApplyRefresh();
            }
        }

        private void ApplyRefresh()
        {
            foreach (var loot in _otherLoot)
            {
                var rigidbody = loot.Rigidbody2D;

                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                float forceMagnitude = Random.Range(1f, 2.5f);
                rigidbody.AddForce(randomDirection * forceMagnitude, ForceMode2D.Impulse);

                float randomTorque = Random.Range(-5f, 5f);
                rigidbody.AddTorque(randomTorque, ForceMode2D.Impulse);
            }
        }
    }
}