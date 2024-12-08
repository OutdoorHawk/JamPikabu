using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class InitLootApplierSystem : IInitializeSystem, ITearDownSystem
    {
        private readonly IGroup<GameEntity> _busyLoot;
        private readonly CancellationTokenSource _tearDown = new();

        public InitLootApplierSystem(GameContext context)
        {
            _busyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Busy
                ));
        }

        public void Initialize()
        {
            

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