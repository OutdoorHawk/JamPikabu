using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ConsumeLootVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly IGroup<GameEntity> _loot;
        private readonly List<GameEntity> _lootBuffer = new(64);

        public ConsumeLootVisualsSystem(GameContext context) : base(context)
        {
            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Consumed
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.LootEffectsApplier,
                GameMatcher.Available).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isLootEffectsApplier && entity.isAvailable;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var applier in entities)
            {
                applier.isAvailable = false;
                AnimateAsync(applier).Forget();
            }
        }

        private async UniTaskVoid AnimateAsync(GameEntity applier)
        {
            foreach (var loot in _loot)
                loot.Retain(this);
            
            await ProcessAnimation();
            
            foreach (var loot in _loot)
                loot.Release(this);

            foreach (var loot in _loot)
                loot.isDestructed = true;

            applier.isDestructed = true;
        }

        private async UniTask ProcessAnimation()
        {
            foreach (var loot in _loot.GetEntities(_lootBuffer))
            {
                await loot.LootItemUI.AnimateConsume();
                
                Debug.Log($"Create remove Withdraw request: {loot.LootTypeId.ToString()} | value: {-loot.GoldValue} ");

                CreateGameEntity.Empty()
                    .With(x => x.isAddGoldRequest = true)
                    .AddGold(0)
                    .AddWithdraw(-loot.GoldValue)
                    ;
            }
        }
    }
}