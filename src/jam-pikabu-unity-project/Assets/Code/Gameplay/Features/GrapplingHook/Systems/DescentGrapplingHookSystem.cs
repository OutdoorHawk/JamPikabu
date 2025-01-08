using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Common.Physics;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.GrapplingHook.Configs;
using Code.Gameplay.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class DescentGrapplingHookSystem : IExecuteSystem
    {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _hooks;
        private readonly IPhysics2DService _physics2DService;
        private readonly IStaticDataService _staticData;

        private readonly GameEntity[] _buffer = new GameEntity[8];
        private readonly List<GameEntity> _hookBuffer = new(2);

        public DescentGrapplingHookSystem(GameContext gameContext, ITimeService time, IPhysics2DService physics2DService, IStaticDataService staticData)
        {
            _physics2DService = physics2DService;
            _staticData = staticData;
            _time = time;
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.Descending,
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
                const float yLimit = -6.12f;

                newPosition.y += moveDownDirection * hook.YAxisDownSpeed * _time.FixedDeltaTime * hook.HookSpeedModifier;

                Transform parent = hookRigidbody2D.transform.parent;
                float minWorldY = parent.TransformPoint(new Vector3(0, yLimit, 0)).y;
                float diff = newPosition.y - currentPosition.y;

                if (CheckReachedMinPosition(newPosition, minWorldY, hook))
                    continue;

                if (CheckCollidedWithLoot(hookRigidbody2D, hook))
                    continue;

                if (CheckTriggeredCollision(hook, diff: diff))
                    continue;

                newPosition.y = Mathf.Max(newPosition.y, minWorldY);
                hookRigidbody2D.MovePosition(newPosition);
            }
        }

        private bool CheckCollidedWithLoot(Rigidbody2D hookRigidbody2D, GameEntity hook)
        {
            Vector2 offset = new Vector2(0, 0.75f);
            
            int hitCount = _physics2DService.CircleCastNonAlloc
            (
                hookRigidbody2D.position - offset,
                hook.StopMovementRaycastRadius,
                CollisionLayer.Loot.AsMask(),
                _buffer
            );

            if (hitCount > 0)
            {
                CompleteDescending(hook);
                return true;
            }

            return false;
        }

        private bool CheckReachedMinPosition(Vector2 newPosition, float minWorldY, GameEntity hook)
        {
            if (newPosition.y <= minWorldY)
            {
                CompleteDescending(hook);
                return true;
            }

            return false;
        }

        private bool CheckTriggeredCollision(GameEntity hook, float diff)
        {
            if (hook.isClosingClaws)
                return true;

            if (hook.GrapplingHookBehaviour.Triggered == false)
                return false;

            if (hook.TriggerMovementThreshold > 0)
            {
                hook.ReplaceTriggerMovementThreshold(hook.TriggerMovementThreshold - Mathf.Abs(diff));
                return false;
            }

            float threshold = _staticData.Get<GrapplingHookStaticData>().TriggerMovementThreshold;
            hook.ReplaceTriggerMovementThreshold(threshold);
            CompleteDescending(hook);
            return true;
        }

        private static void CompleteDescending(GameEntity hook)
        {
            hook.isDescending = false;
            hook.GrapplingHookBehaviour.CloseClawsAndReturn();
        }
    }
}