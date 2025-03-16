using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    namespace Code.Gameplay.Features.Loot.Systems
    {
        public sealed class LootIngredientPickupLogicSystem : IExecuteSystem
        {
            private readonly IGameplayLootService _gameplayLootService;
            private readonly IOrdersService _ordersService;

            private readonly IGroup<GameEntity> _timers;
            private readonly IGroup<GameEntity> _lootToCollect;

            private readonly List<GameEntity> _buffer = new(16);

            public LootIngredientPickupLogicSystem
            (
                GameContext context,
                IGameplayLootService gameplayLootService,
                IOrdersService ordersService
            )
            {
                _gameplayLootService = gameplayLootService;
                _ordersService = ordersService;

                _timers = context.GetGroup(GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess));

                _lootToCollect = context.GetGroup(GameMatcher
                    .AllOf(
                        GameMatcher.Loot,
                        GameMatcher.LootTypeId,
                        GameMatcher.CollectLootRequest
                    ));
            }

            public void Execute()
            {
                foreach (var loot in _lootToCollect.GetEntities(_buffer))
                {
                    SetLootCollected(loot);
                    TryCompleteTimerIfAllLootCollected();
                    DisableCollisions(loot);
                }

                _buffer.Clear();
            }

            private void SetLootCollected(GameEntity loot)
            {
                _gameplayLootService.AddCollectedLoot(loot.LootTypeId, ratingAmount: loot.hasRating ? loot.Rating : 0);
                loot.isCollected = true;
            }

            private void TryCompleteTimerIfAllLootCollected()
            {
                if (_ordersService.GetOrderProgress() >= 1)
                {
                    foreach (var timer in _timers)
                    {
                        timer.isRoundEndRequest = true;
                    }
                }
            }

            private static void DisableCollisions(GameEntity loot)
            {
                foreach (var collider in loot.Rigidbody2D.GetComponentsInChildren<Collider2D>())
                {
                    collider.enabled = false;
                }
            }
        }
    }
}