using System.Collections.Generic;
using Code.Gameplay.Common.Collisions;
using UnityEngine;

namespace Code.Gameplay.Common.Physics
{
    public class PhysicsService : IPhysicsService
    {
        private static readonly RaycastHit[] Hits = new RaycastHit[128];
        private static readonly Collider[] OverlapHits = new Collider[128];

        private readonly ICollisionRegistry _collisionRegistry;

        public PhysicsService(ICollisionRegistry collisionRegistry)
        {
            _collisionRegistry = collisionRegistry;
        }

        public int BoxCastNonAlloc(Vector3 boxCenter, Vector3 direction, Vector3 halfExtends, Quaternion boxRotation, GameEntity[] hitBuffer, LayerMask mask)
        {
            int hitCount = UnityEngine.Physics.BoxCastNonAlloc
            (
                boxCenter, 
                halfExtends,
                direction, 
                Hits,
                boxRotation,
                1,
                mask
            );

            DrawWireframeBox(boxCenter, halfExtends, boxRotation, 0.5f);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = Hits[i];

                if (hit.collider == null)
                    continue;

                GameEntity entity = _collisionRegistry.Get<GameEntity>(hit.collider.GetInstanceID());

                if (entity == null)
                    continue;

                if (i < hitBuffer.Length)
                    hitBuffer[i] = entity;
            }
            
            return hitCount;
        }

        public IEnumerable<GameEntity> RaycastAll(Vector3 worldPosition, Vector3 direction, int layerMask)
        {
            int hitCount = UnityEngine.Physics.RaycastNonAlloc(worldPosition, direction, Hits, layerMask);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = Hits[i];

                if (hit.collider == null)
                    continue;

                GameEntity entity = _collisionRegistry.Get<GameEntity>(hit.collider.GetInstanceID());

                if (entity == null)
                    continue;

                yield return entity;
            }
        }

        public int RaycastNonAlloc(Vector3 worldPosition, Vector3 direction, int layerMask, GameEntity[] hitBuffer,
            QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, List<RaycastHit> hitsBuffer = null)
        {
            int hitCount = UnityEngine.Physics.RaycastNonAlloc(worldPosition, direction, Hits, direction.magnitude, layerMask, triggerInteraction);
            hitsBuffer?.Clear();

            Debug.DrawRay(worldPosition, direction, Color.green, 1);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = Hits[i];

                if (hit.collider == null)
                    continue;

                GameEntity entity = _collisionRegistry.Get<GameEntity>(hit.collider.GetInstanceID());

                if (entity == null)
                    continue;

                if (i < hitBuffer.Length)
                    hitBuffer[i] = entity;
                
                hitsBuffer?.Add(hit);
            }

            return hitCount;
        }

        public GameEntity LineCast(Vector3 start, Vector3 end, int layerMask)
        {
            int hitCount = UnityEngine.Physics.RaycastNonAlloc(start, end, Hits, layerMask);

            Debug.DrawLine(start, end, Color.green, 1);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = Hits[i];
                if (hit.collider == null)
                    continue;

                GameEntity entity = _collisionRegistry.Get<GameEntity>(hit.collider.GetInstanceID());
                if (entity == null)
                    continue;

                return entity;
            }

            return null;
        }

        public IEnumerable<GameEntity> SphereCast(Vector3 position, float radius, int layerMask)
        {
            int hitCount = OverlapSphere(position, radius, OverlapHits, layerMask);

            DrawDebugSphere(position, radius, Color.red, 1f);

            for (int i = 0; i < hitCount; i++)
            {
                GameEntity entity = _collisionRegistry.Get<GameEntity>(OverlapHits[i].GetInstanceID());

                if (entity == null)
                    continue;

                yield return entity;
            }
        }

        public int SphereCastNonAlloc(Vector3 position, float radius, int layerMask, GameEntity[] hitBuffer)
        {
            int hitCount = OverlapSphere(position, radius, OverlapHits, layerMask);

            DrawDebug(position, radius, 1f, Color.green);

            for (int i = 0; i < hitCount; i++)
            {
                GameEntity entity = _collisionRegistry.Get<GameEntity>(OverlapHits[i].GetInstanceID());

                if (entity == null)
                    continue;

                if (i < hitBuffer.Length)
                    hitBuffer[i] = entity;
            }

            return hitCount;
        }

        public int OverlapSphere(Vector3 worldPos, float radius, Collider[] hits, int layerMask)
        {
            return UnityEngine.Physics.OverlapSphereNonAlloc(worldPos, radius, hits, layerMask);
        }

        #region Debug

        public static void DrawDebug(Vector3 worldPos, float radius, float seconds, Color color)
        {
            Debug.DrawRay(worldPos, radius * Vector3.up, color, seconds);
            Debug.DrawRay(worldPos, radius * Vector3.down, color, seconds);
            Debug.DrawRay(worldPos, radius * Vector3.left, color, seconds);
            Debug.DrawRay(worldPos, radius * Vector3.right, color, seconds);
        }

        public static void DrawDebugSphere(Vector3 center, float radius, Color color, float duration)
        {
            const int segments = 10; // Adjust the number of segments for smoother appearance

            // Draw lines along latitude
            for (int i = 0; i <= segments; i++)
            {
                float theta = Mathf.PI * i / segments;
                float y = Mathf.Cos(theta) * radius;
                float r = Mathf.Sin(theta) * radius;

                // Draw lines along longitude
                for (int j = 0; j < segments; j++)
                {
                    float phi = 2 * Mathf.PI * j / segments;
                    Vector3 point1 = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r) + center;
                    Vector3 point2 = new Vector3(Mathf.Cos(phi + 2 * Mathf.PI / segments) * r, y, Mathf.Sin(phi + 2 * Mathf.PI / segments) * r) + center;

                    Debug.DrawLine(point1, point2, color, duration);
                }
            }
        }
        
        public static void DrawWireframeBox(Vector3 center, Vector3 halfExtents, Quaternion rotation, float duration)
        {
            Vector3[] vertices = GetBoxVertices(center, halfExtents, rotation);

            // Draw the lines of the wireframe box
            for (int i = 0; i < 4; i++)
            {
                int nextIndex = (i + 1) % 4;
                Debug.DrawLine(vertices[i], vertices[nextIndex], Color.red, duration);
                Debug.DrawLine(vertices[i + 4], vertices[nextIndex + 4], Color.red, duration);
                Debug.DrawLine(vertices[i], vertices[i + 4], Color.red, duration);
            }

            Debug.DrawLine(vertices[0], vertices[1], Color.red, duration);
            Debug.DrawLine(vertices[1], vertices[2], Color.red, duration);
            Debug.DrawLine(vertices[2], vertices[3], Color.red, duration);
            Debug.DrawLine(vertices[3], vertices[0], Color.red, duration);
        }
        
        private static Vector3[] GetBoxVertices(Vector3 center, Vector3 halfExtents, Quaternion rotation)
        {
            // Calculate the local axes of the box based on its rotation
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;
            Vector3 forward = rotation * Vector3.forward;

            // Calculate the corner points of the box in local space
            Vector3[] localVertices = 
            {
                center + right * halfExtents.x + up * halfExtents.y + forward * halfExtents.z,
                center + right * halfExtents.x + up * halfExtents.y - forward * halfExtents.z,
                center + right * halfExtents.x - up * halfExtents.y - forward * halfExtents.z,
                center + right * halfExtents.x - up * halfExtents.y + forward * halfExtents.z,
                center - right * halfExtents.x + up * halfExtents.y + forward * halfExtents.z,
                center - right * halfExtents.x + up * halfExtents.y - forward * halfExtents.z,
                center - right * halfExtents.x - up * halfExtents.y - forward * halfExtents.z,
                center - right * halfExtents.x - up * halfExtents.y + forward * halfExtents.z
            };

            return localVertices;
        }

        #endregion
    }
}