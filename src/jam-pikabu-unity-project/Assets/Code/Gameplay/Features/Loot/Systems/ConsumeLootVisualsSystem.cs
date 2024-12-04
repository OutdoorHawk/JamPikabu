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
        private readonly IGroup<GameEntity> _lootApplier;
        private readonly IGroup<GameEntity> _loot;
        private readonly List<GameEntity> _lootBuffer = new(64);

        public ConsumeLootVisualsSystem(GameContext context) : base(context)
        {
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));

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
            ;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _lootBuffer.Clear();
            _loot.GetEntities(_lootBuffer);
            
            foreach (var applier in _lootApplier)
                applier.isAvailable = false;
            
            AnimateAsync().Forget();
        }

        private async UniTaskVoid AnimateAsync()
        {
            await ProcessAnimation();

            foreach (var loot in _lootBuffer)
                loot.isDestructed = true;

            _lootBuffer.Clear();

            foreach (var applier in _lootApplier)
                applier.isDestructed = true;
        }

        private async UniTask ProcessAnimation()
        {
            foreach (var loot in _lootBuffer)
            {
                await loot.LootItemUI.AnimateConsume();

                CreateGameEntity.Empty()
                    .With(x => x.isAddGoldRequest = true)
                    .AddGold(0)
                    .AddWithdraw(-loot.GoldValue)
                    ;
            }
        }
    }
}