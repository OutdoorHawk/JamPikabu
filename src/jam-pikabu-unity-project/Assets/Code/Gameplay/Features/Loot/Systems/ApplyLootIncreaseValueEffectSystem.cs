using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootIncreaseValueEffectSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _lootProducer;
        private readonly IGroup<GameEntity> _potentialTargets;

        private readonly List<GameEntity> _lootBuffer = new(64);
        private readonly List<GameEntity> _targetsBuffer = new(64);
        private readonly IGroup<GameEntity> _lootApplier;

        public ApplyLootIncreaseValueEffectSystem(GameContext context)
        {
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier,
                    GameMatcher.EffectApplicationAvailable
                ));
            
            _lootProducer = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.LootItemUI,
                    GameMatcher.EffectValue,
                    GameMatcher.EffectTargetsLoot
                ));
            
            _potentialTargets = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.LootItemUI,
                    GameMatcher.LootTypeId,
                    GameMatcher.GoldValue
                ));
        }

        public void Execute()
        {
            foreach (var _ in _lootApplier)
            {
                foreach (var producer in _lootProducer.GetEntities(_lootBuffer))
                foreach (var target in _potentialTargets)
                {
                    if (producer.EffectTargetsLoot.Contains(target.LootTypeId) == false)
                        continue;

                    producer.isBusy = true;
                    target.isBusy = true;
                    
                    target.LootItemUI.SetGoldValueWithdraw((int)producer.EffectValue);
                    target.ReplaceGold((int)(target.GoldValue + producer.EffectValue));
                }
                
                ApplyAsync().Forget();
            }
        }

        private async UniTaskVoid ApplyAsync()
        {
            foreach (var producer in _lootProducer.GetEntities(_lootBuffer))
            foreach (var target in _potentialTargets.GetEntities(_targetsBuffer))
            {
                if (producer.EffectTargetsLoot.Contains(target.LootTypeId) == false)
                    continue;

                producer.LootItemUI.AnimateEffectProducer().Forget();
                await target.LootItemUI.AnimateEffectTarget();
                target.LootItemUI.SetGoldValueWithdraw(0);
            }
        }
    }
}