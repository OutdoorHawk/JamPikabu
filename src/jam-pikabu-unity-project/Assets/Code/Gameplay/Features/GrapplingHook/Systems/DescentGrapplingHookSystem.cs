using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Common.Physics;
using Code.Gameplay.Common.Time;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class DescentGrapplingHookSystem : IExecuteSystem
    {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _hooks;
        private readonly IPhysics2DService _physics2DService;
        
        private readonly GameEntity[] _buffer = new GameEntity[8];
        private readonly List<GameEntity> _hookBuffer = new(2);

        public DescentGrapplingHookSystem(GameContext gameContext, ITimeService time, IPhysics2DService physics2DService)
        {
            _physics2DService = physics2DService;
            _time = time;
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.DescentRequested,
                    GameMatcher.DescentAvailable,
                    GameMatcher.Rigidbody2D,
                    GameMatcher.GrapplingHookBehaviour
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks.GetEntities(_hookBuffer))
            {
                Rigidbody2D hookRigidbody2D = hook.Rigidbody2D;
                Vector2 currentPosition = hookRigidbody2D.position;
                Vector2 newPosition = currentPosition;

                const float moveDownDirection = -1;
                const float yLimit = -5.6f;
                
                newPosition.y += moveDownDirection * hook.YAxisDownSpeed * _time.FixedDeltaTime;
                
                Transform parent = hookRigidbody2D.transform.parent;
                float minWorldY = parent.TransformPoint(new Vector3(0, yLimit, 0)).y;
                
                newPosition.y = Mathf.Max(newPosition.y, minWorldY);
                
                hookRigidbody2D.MovePosition(newPosition);
                
                if (CheckReachedMinPosition(newPosition, minWorldY, hook)) 
                    continue;

                if (CheckCollidedWithLoot(hookRigidbody2D, hook)) 
                    continue;
            }
        }

        private bool CheckCollidedWithLoot(Rigidbody2D hookRigidbody2D, GameEntity hook)
        {
            int hitCount = _physics2DService.CircleCastNonAlloc
            (
                hookRigidbody2D.position,
                hook.StopMovementRaycastRadius,
                CollisionLayer.Loot.AsMask(),
                _buffer
            );

            if (hitCount > 0)
            {
                hook.isDescentRequested = false;
                hook.GrapplingHookBehaviour.CloseClawsAndReturn();
                return true;
            }

            return false;
        }

        private static bool CheckReachedMinPosition(Vector2 newPosition, float minWorldY, GameEntity hook)
        {
            if (newPosition.y <= minWorldY)
            {
                hook.isDescentRequested = false;
                hook.GrapplingHookBehaviour.CloseClawsAndReturn();
                return true;
            }

            return false;
        }
    }
}