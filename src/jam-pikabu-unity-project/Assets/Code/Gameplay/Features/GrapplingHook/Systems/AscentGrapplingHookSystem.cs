using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class AscentGrapplingHookSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _hooks;
        private readonly ITimeService _time;
        private readonly List<GameEntity> _buffer = new(2);

        public AscentGrapplingHookSystem(GameContext gameContext, ITimeService time)
        {
            _time = time;
            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.AscentRequested,
                    GameMatcher.AscentAvailable,
                    GameMatcher.Rigidbody2D,
                    GameMatcher.GrapplingHookBehaviour
                ));
        }

        public void Execute()
        {
            foreach (var hook in _hooks.GetEntities(_buffer))
            {
                Rigidbody2D hookRigidbody2D = hook.Rigidbody2D;
                Vector2 currentPosition = hookRigidbody2D.position;
                Vector2 newPosition = currentPosition;

                const float moveUpDirection = 1;
                const float yLimit = 0;

                newPosition.y += moveUpDirection * hook.YAxisUpSpeed * _time.FixedDeltaTime;

                Transform parent = hookRigidbody2D.transform.parent;
                float maxWorldY = parent.TransformPoint(new Vector3(0, yLimit, 0)).y;

                newPosition.y = Mathf.Min(newPosition.y, maxWorldY);
                hookRigidbody2D.MovePosition(newPosition);

                if (newPosition.y >= maxWorldY)
                {
                    hook.isAscentAvailable = false;
                    hook.isAscentRequested = false;
                    hook.isCollectLootRequest = true;
                }
            }
        }
    }
}