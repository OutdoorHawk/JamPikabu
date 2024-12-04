using System.Collections.Generic;
using Code.Common;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootIncreaseValueEffectVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly List<GameEntity> _producers = new(64);
        private readonly GameContext _context;

        private readonly IGroup<GameEntity> _lootApplier;
        private readonly IGroup<GameEntity> _loot;

        public ApplyLootIncreaseValueEffectVisualsSystem(GameContext context) : base(context)
        {
            _context = context;

            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));

            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.IncreaseValueEffect,
                    GameMatcher.Targets,
                    GameMatcher.Applied));
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
                applier.isAvailable = false;

            _producers.Clear();
            _loot.GetEntities(_producers);
            ApplyAsync().Forget();
        }

        private async UniTaskVoid ApplyAsync()
        {
            await ProcessAnimation();

            _producers.Clear();

            foreach (var applier in _lootApplier)
                applier.isAvailable = true;
        }

        private async UniTask ProcessAnimation()
        {
            foreach (var producer in _producers)
            {
                foreach (int targetId in producer.Targets)
                {
                    GameEntity target = _context.GetEntityWithId(targetId);

                    if (target.IsNullOrDestructed())
                        continue;

                    PlayEffect(producer.EffectValue, target).Forget();
                }

                await producer.LootItemUI.AnimateEffectProducer();
                producer.RemoveTargets();
            }
        }

        private async UniTaskVoid PlayEffect(float effectValue, GameEntity target)
        {
            await target.LootItemUI.AnimateEffectTarget();
            target.LootItemUI.AddGoldValueWithdraw((int)-effectValue);
        }
    }
}