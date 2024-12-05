using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootIncreaseValueEffectSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _lootProducer;
        private readonly IGroup<GameEntity> _potentialTargets;

        private readonly List<GameEntity> _lootBuffer = new(64);
        private readonly IGroup<GameEntity> _lootApplier;

        public ApplyLootIncreaseValueEffectSystem(GameContext context)
        {
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));

            _lootProducer = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.IncreaseValueEffect,
                    GameMatcher.EffectValue,
                    GameMatcher.LootItemUI,
                    GameMatcher.EffectTargetsLoot
                ).NoneOf(
                    GameMatcher.Applied));

            _potentialTargets = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.LootTypeId,
                    GameMatcher.LootItemUI,
                    GameMatcher.GoldValue
                )
            );
        }

        public void Execute()
        {
            foreach (var _ in _lootApplier)
            foreach (var producer in _lootProducer.GetEntities(_lootBuffer))
            {
                foreach (var target in _potentialTargets)
                {
                    if (producer.Id == target.Id)
                        continue;

                    if (producer.EffectTargetsLoot.Contains(target.LootTypeId) == false)
                        continue;

                    target.ReplaceGoldValue((int)(target.GoldValue + producer.EffectValue));
                    Debug.Log($"boost: {target.LootTypeId.ToString()} | effect: {producer.EffectValue} | result gold {target.GoldValue}");
                    target.LootItemUI.AddGoldValueWithdraw((int)producer.EffectValue);
                    producer.Targets.Add(target.Id);
                }

                producer.isApplied = producer.Targets.Count > 0;
            }
        }
    }
}