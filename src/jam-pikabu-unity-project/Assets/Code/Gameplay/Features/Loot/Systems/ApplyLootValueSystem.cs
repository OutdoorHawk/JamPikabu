using System.Collections.Generic;
using System.Linq;
using Code.Common.Entity;
using Code.Common.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootValueSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundOver;
        private readonly IGroup<GameEntity> _readyLoot;
        private readonly IGroup<GameEntity> _collectedLoot;
        
        private readonly List<GameEntity> _buffer = new(64);

        public ApplyLootValueSystem(GameContext context)
        {
            _roundOver = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundOver));

            _collectedLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected
                ));

            _readyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.LootItemUI,
                    GameMatcher.ReadyToApply,
                    GameMatcher.GoldValue
                ));
        }

        public void Execute()
        {
            foreach (var _ in _roundOver)
            {
                if (CheckLootIsStillBusy())
                    continue;

                foreach (var loot in _readyLoot.GetEntities(_buffer))
                {
                    CreateGameEntity.Empty()
                        .With(x => x.isAddGoldRequest = true)
                        .AddGold(loot.GoldValue.Amount)
                        ;

                    loot.isReadyToApply = false;
                    loot.isApplied = true;
                }
            }
        }

        private bool CheckLootIsStillBusy()
        {
            return _collectedLoot
                .GetEntities()
                .All(x => x.isReadyToApply) == false;
        }
    }
}