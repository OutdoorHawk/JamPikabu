using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ConsumeLootVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly IGroup<GameEntity> _lootApplier;
        private readonly List<GameEntity> _buffer = new(64);

        public ConsumeLootVisualsSystem(GameContext context) : base(context)
        {
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.Loot,
                GameMatcher.Consumed).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isLoot && entity.isConsumed && entity.hasLootItemUI;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _buffer.Clear();
            _buffer.AddRange(entities);

            AnimateAsync().Forget();
        }

        private async UniTaskVoid AnimateAsync()
        {
            await ProcessAnimation();

            foreach (var loot in _buffer)
                loot.isDestructed = true;

            _buffer.Clear();

            foreach (var applier in _lootApplier)
                applier.isDestructed = true;
        }

        private async UniTask ProcessAnimation()
        {
            foreach (var loot in _buffer)
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