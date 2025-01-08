using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Common.Physics;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class MarkLootAscendingInsideHookSystem : IExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<GameEntity> _hooks;
        private readonly IGroup<GameEntity> _lootInHook;
        
        private readonly IPhysics2DService _physics2DService;
        
        private readonly GameEntity[] _raycastBuffer = new GameEntity[16];
        private readonly List<GameEntity> _buffer = new(16);

        public MarkLootAscendingInsideHookSystem(GameContext gameContext, IPhysics2DService physics2DService)
        {
            _physics2DService = physics2DService;
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.Ascending,
                    GameMatcher.Rigidbody2D,
                    GameMatcher.GrapplingHookBehaviour
                ));

            _lootInHook = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.InsideHook
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks)
            {
                _raycastBuffer.ClearArray();

                const float hookCenterRadius = 0.75f;

                int hitCount = _physics2DService.CircleCastNonAlloc
                (
                    hook.GrapplingHookBehaviour.HookCenter.position,
                    hookCenterRadius,
                    CollisionLayer.Loot.AsMask(),
                    _raycastBuffer
                );

                if (hitCount == 0)
                    continue;

                for (int i = 0; i < _raycastBuffer.Length; i++)
                {
                    if (_raycastBuffer[i] == null)
                        continue;
                    
                    _raycastBuffer[i].isInsideHook = true;
                }
            }
        }

        public void Cleanup()
        {
            foreach (GameEntity gameEntity in _lootInHook.GetEntities(_buffer))
            {
                gameEntity.isInsideHook = false;
            }
        }
    }
}