using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Consumables.Systems
{
    public class ActivateSpoonConsumableSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _otherLoot;

        public ActivateSpoonConsumableSystem(GameContext context)
        {
            _requests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.ActivateConsumableRequest,
                    GameMatcher.Spoon,
                    GameMatcher.Processed
                ));

            _otherLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.Rigidbody2D
                ).NoneOf(
                    GameMatcher.MarkedForPickup));
        }

        public void Execute()
        {
            foreach (var _ in _requests)
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