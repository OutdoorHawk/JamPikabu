using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ConsumeLootValueSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _readyLoot;
        private readonly IGroup<GameEntity> _lootApplier;
        private readonly List<GameEntity> _buffer = new(64);

        public ConsumeLootValueSystem(GameContext context)
        {
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier,
                    GameMatcher.LootConsumeAvailable
                ));

            _readyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.LootItemUI,
                    GameMatcher.GoldValue
                ));
        }

        public void Execute()
        {
            foreach (var _ in _lootApplier)
            {
                foreach (var loot in _readyLoot)
                {
                    loot.isConsumed = true;
                    
                    CreateGameEntity.Empty()
                        .With(x => x.isAddGoldRequest = true)
                        .AddGold(loot.GoldValue)
                        ;
                }

                //GiveGoldAsync(_readyLoot).Forget();
            }
        }

        private async UniTaskVoid GiveGoldAsync(IGroup<GameEntity> readyLoot)
        {
            foreach (var loot in readyLoot.GetEntities(_buffer))
            {
                loot.Retain(this);
                await loot.LootItemUI.AnimateConsume();
                
                loot.isDestructed = true;
                loot.Release(this);
            }
        }
    }
}