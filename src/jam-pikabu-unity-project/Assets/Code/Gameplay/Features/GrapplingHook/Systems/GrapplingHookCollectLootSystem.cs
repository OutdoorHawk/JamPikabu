using System.Collections.Generic;
using System.Threading;
using Code.Common;
using Code.Common.Extensions;
using Code.Gameplay.Common.Physics;
using Cysharp.Threading.Tasks;
using Entitas;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class GrapplingHookCollectLootSystem : IExecuteSystem, ITearDownSystem
    {
        private readonly IPhysics2DService _physics2DService;
        private readonly IGroup<GameEntity> _hooks;

        private readonly GameEntity[] _buffer = new GameEntity[32];
        private readonly List<GameEntity> _bufferEntity = new(2);

        private readonly CancellationTokenSource _tearDownToken = new();

        public GrapplingHookCollectLootSystem(GameContext context, IPhysics2DService physics2DService)
        {
            _physics2DService = physics2DService;
            _hooks = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.GrapplingHook,
                    GameMatcher.CollectLootRequest,
                    GameMatcher.Rigidbody2D,
                    GameMatcher.CollectLootRaycastRadius,
                    GameMatcher.CollectLootPieceInterval
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks.GetEntities(_bufferEntity))
            {
                hook.isCollectLootRequest = false;
                hook.isCollectingLoot = true;

                CollectLootAsync(hook).Forget();
            }
        }

        public void TearDown()
        {
            _tearDownToken?.Cancel();
        }

        private async UniTaskVoid CollectLootAsync(GameEntity hook)
        {
            hook.Retain(this);

            _buffer.ClearArray();

            int hitCount = _physics2DService.CircleCastNonAlloc
            (
                hook.Rigidbody2D.position,
                hook.CollectLootRaycastRadius,
                CollisionLayer.Loot.AsMask(),
                _buffer
            );

            if (hitCount != 0)
            {
                foreach (var loot in _buffer)
                {
                    if (loot.IsNullOrDestructed())
                        continue;

                    loot.isMarkedForPickup = true;
                }

                foreach (var loot in _buffer)
                {
                    if (loot.IsNullOrDestructed())
                        continue;
                    
                    if (loot.isMarkedForPickup == false)
                        continue;

                    loot.isCollectLootRequest = true;
                    loot.Retain(this);
                    await DelaySeconds(hook.CollectLootPieceInterval, _tearDownToken.Token);
                    loot.Release(this);
                }
            }

            hook.isCollectingLoot = false;
            hook.GrapplingHookBehaviour.OpenClaws();
            hook.Release(this);
        }
    }
}