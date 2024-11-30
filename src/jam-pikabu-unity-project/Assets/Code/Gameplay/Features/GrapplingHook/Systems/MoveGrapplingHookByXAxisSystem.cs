using Code.Gameplay.Common.Time;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class MoveGrapplingHookByXAxisSystem : IExecuteSystem
    {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _hooks;

        public MoveGrapplingHookByXAxisSystem(GameContext gameContext, ITimeService time)
        {
            _time = time;
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.XAxisMoveDirection,
                    GameMatcher.XAxisMovementAvailable,
                    GameMatcher.Rigidbody2D
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks)
            {
                Rigidbody2D hookRigidbody2D = hook.Rigidbody2D;
                Vector2 currentPosition = hookRigidbody2D.position;
                Vector2 newPosition = currentPosition;
                newPosition.x += hook.XAxisMoveDirection * hook.XAxisSpeed * _time.FixedDeltaTime;
                
                Transform parent = hookRigidbody2D.transform.parent;
                float minWorldX = parent.TransformPoint(new Vector3(hook.XMovementLimits.x, 0, 0)).x;
                float maxWorldX = parent.TransformPoint(new Vector3(hook.XMovementLimits.y, 0, 0)).x;
                
                newPosition.x = Mathf.Clamp(newPosition.x, minWorldX, maxWorldX);
                
                hookRigidbody2D.MovePosition(newPosition);
            }
        }
    }
}