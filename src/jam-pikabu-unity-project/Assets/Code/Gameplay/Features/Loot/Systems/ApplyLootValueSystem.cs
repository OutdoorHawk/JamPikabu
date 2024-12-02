using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Infrastructure.Systems;
using Cysharp.Threading.Tasks;
using Entitas;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootValueSystem : BufferedExecuteSystem, ITearDownSystem
    {
        private readonly IGroup<GameEntity> _lootApplier;
        private readonly IGroup<GameEntity> _collectedLoot;
        private readonly IGroup<GameEntity> _readyLoot;
        
        private readonly List<GameEntity> _lootBuffer = new(64);
        
        private readonly CancellationTokenSource _teardownCancellationTokenSource = new();

        public ApplyLootValueSystem(GameContext context)
        {
            _lootApplier = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.LootEffectsApplier,
                    GameMatcher.ReadyToApplyValues));

            _collectedLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected
                ));
            
            _readyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.LootItemUI,
                    GameMatcher.ReadyToApply,
                    GameMatcher.GoldValue
                ));
        }

        public override void Execute()
        {
            foreach (var applier in _lootApplier.GetEntities(_buffer))
            {
                if (CheckLootIsStillBusy())
                    continue;

                applier.isReadyToApplyValues = false;
                applier.isApplyingValues = true;

                ApplyLootValues(applier).Forget();
            }
        }

        public void TearDown()
        {
            _teardownCancellationTokenSource?.Cancel();
        }

        private bool CheckLootIsStillBusy()
        {
            return _collectedLoot
                .GetEntities()
                .All(x => x.isReadyToApply) == false;
        }

        private async UniTaskVoid ApplyLootValues(GameEntity applier)
        {
            foreach (var loot in _readyLoot.GetEntities(_lootBuffer))
            {
                CreateGameEntity.Empty()
                    .With(x => x.isAddGoldRequest = true)
                    .AddGold(loot.GoldValue.Amount)
                    ;

                loot.isReadyToApply = false;
                loot.isApplied = true;

                await DelaySeconds(0.5f, _teardownCancellationTokenSource.Token);
            }
            
            await DelaySeconds(1, _teardownCancellationTokenSource.Token);
            applier.isApplyingValues = false;
            applier.isDestructed = true;
        }
    }
}