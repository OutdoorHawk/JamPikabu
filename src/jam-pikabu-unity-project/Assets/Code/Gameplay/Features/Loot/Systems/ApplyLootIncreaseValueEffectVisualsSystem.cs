using System.Collections.Generic;
using Code.Common;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootIncreaseValueEffectVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        private readonly IGroup<GameEntity> _loot;

        private readonly List<GameEntity> _buffer = new(64);

        public ApplyLootIncreaseValueEffectVisualsSystem(GameContext context) : base(context)
        {
            _context = context;

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
            {
                applier.isAvailable = false;
                ApplyAsync(applier).Forget();
            }
        }

        private async UniTaskVoid ApplyAsync(GameEntity applier)
        {
            foreach (var loot in _loot)
                loot.Retain(this);
            
            await ProcessAnimation();
            
            foreach (var loot in _loot)
                loot.Release(this);

            applier.isAvailable = true;
        }

        private async UniTask ProcessAnimation()
        {
            foreach (var producer in _loot.GetEntities(_buffer))
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