using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Common.Physics
{
    public interface IPhysicsService
    {
        int BoxCastNonAlloc(Vector3 boxCenter, Vector3 direction, Vector3 halfExtends, Quaternion boxRotation, GameEntity[] hitBuffer, LayerMask mask);
        IEnumerable<GameEntity> RaycastAll(Vector3 worldPosition, Vector3 direction, int layerMask);
        GameEntity LineCast(Vector3 start, Vector3 end, int layerMask);
        IEnumerable<GameEntity> SphereCast(Vector3 position, float radius, int layerMask);
        int SphereCastNonAlloc(Vector3 position, float radius, int layerMask, GameEntity[] hitBuffer);
        int OverlapSphere(Vector3 worldPos, float radius, Collider[] hits, int layerMask);
        int RaycastNonAlloc(Vector3 worldPosition, Vector3 direction, int layerMask, GameEntity[] hitBuffer,
            QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, List<RaycastHit> hitsBuffer = null);
    }
}