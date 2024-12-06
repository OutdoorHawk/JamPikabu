using System.Collections.Generic;
using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class CreateLootApplierOnRoundOverSystem : ReactiveSystem<GameEntity>, ITearDownSystem
    {
        private readonly IGroup<GameEntity> _busyLoot;
        private readonly CancellationTokenSource _tearDown = new();

        public CreateLootApplierOnRoundOverSystem(GameContext context) : base(context)
        {
            _busyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Busy
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.RoundStateController,
                GameMatcher.RoundOver).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            CreateGameEntity
                .Empty()
                .With(x => x.isLootEffectsApplier = true)
                .With(x => x.isAvailable = true)
                ;
            
            CreateAsync().Forget();
        }

        private async UniTask CreateAsync()
        {
            await UniTask.WaitUntil(() => _busyLoot.GetEntities().Length == 0, cancellationToken: _tearDown.Token);
        }

        public void TearDown()
        {
            _tearDown?.Cancel();
        }
    }
}