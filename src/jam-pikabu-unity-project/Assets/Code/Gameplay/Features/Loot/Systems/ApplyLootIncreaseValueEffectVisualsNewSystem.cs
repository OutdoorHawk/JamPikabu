using System.Collections.Generic;
using Code.Common;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootIncreaseValueEffectVisualsNewSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        private readonly IGroup<GameEntity> _increaseValueEffects;

        private readonly List<GameEntity> _buffer = new(64);

        public ApplyLootIncreaseValueEffectVisualsNewSystem(GameContext context) : base(context)
        {
            _context = context;

            _increaseValueEffects = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Effect,
                    GameMatcher.IncreaseValueEffect,
                    GameMatcher.EffectValue,
                    GameMatcher.Targets));
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
            foreach (var loot in _increaseValueEffects)
                loot.Retain(this);
            
            await ProcessAnimation();
            
            foreach (var loot in _increaseValueEffects)
                loot.Release(this);

            applier.isAvailable = true;
        }

        private async UniTask ProcessAnimation()
        {
            foreach (var effect in _increaseValueEffects.GetEntities(_buffer))
            {
                foreach (int targetId in effect.Targets)
                {
                    GameEntity target = _context.GetEntityWithId(targetId);

                    if (target.IsNullOrDestructed())
                        continue;

                    PlayEffect(effect.EffectValue, target).Forget();
                }
                
                GameEntity producer = _context.GetEntityWithId(effect.Producer);

                if (producer.hasLootItemUI) 
                    await producer.LootItemUI.AnimateEffectProducer();

                effect.isDestructed = true;
            }
        }

        private async UniTaskVoid PlayEffect(float effectValue, GameEntity target)
        {
            await target.LootItemUI.AnimateEffectTarget();
            target.LootItemUI.AddGoldValueWithdraw((int)-effectValue);
        }
    }
}