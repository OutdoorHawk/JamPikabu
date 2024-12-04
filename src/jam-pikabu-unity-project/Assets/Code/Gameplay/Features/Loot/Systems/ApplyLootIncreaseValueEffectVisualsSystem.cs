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

        public ApplyLootIncreaseValueEffectVisualsSystem(GameContext context) : base(context)
        {
            _context = context;
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.Loot,
                GameMatcher.IncreaseValueEffect,
                GameMatcher.Targets,
                GameMatcher.Applied).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isLoot && entity.isIncreaseValueEffect && entity.hasTargets && entity.isApplied;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _producers.Clear();
            _producers.AddRange(entities);

            ApplyAsync().Forget();
        }

        private async UniTaskVoid ApplyAsync()
        {
            await ProcessAnimation();
            
            _producers.Clear();

            foreach (var applier in _lootApplier)
                applier.isDestructed = true;
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

                    PlayEffect(target).Forget();
                }

                await producer.LootItemUI.AnimateEffectProducer();
            }
        }

        private async UniTaskVoid PlayEffect(GameEntity target)
        {
            await target.LootItemUI.AnimateEffectTarget();
            target.LootItemUI.SetGoldValueWithdraw(0);
        }
    }
}