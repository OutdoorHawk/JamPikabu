using System.Collections.Generic;

using UnityEngine;

namespace Code.Common.Extensions
{
    public static class CombatExtensions
    {
        private static readonly Collider[] _colliders = new Collider[30];

        public static Collider[] OverlapInRange(Transform origin, float overlapRadius, LayerMask detectMask, Collider excludeCollider = null)
        {
            Extensions.ClearArray(_colliders);
            int size = Physics.OverlapSphereNonAlloc(origin.position, overlapRadius, _colliders, detectMask, QueryTriggerInteraction.Ignore);
            ExcludeSelf(excludeCollider, _colliders);
            return size != 0 ? _colliders : null;
        }

        public static Collider CollectNearestTargetInRange(Transform origin, float overlapRadius, LayerMask detectMask, Collider excludeCollider)
        {
            OverlapInRange(origin, overlapRadius, detectMask, excludeCollider);
            ExcludeSelf(excludeCollider, _colliders);
            return _colliders.Length != 0
                ? GetNearestTargetInRange(origin, _colliders, overlapRadius)
                : null;
        }

        private static Collider GetNearestTargetInRange(Transform origin, Collider[] targets, float attackRange)
        {
            float minDistance = attackRange;
            Collider nearestTarget = null;

            foreach (Collider target in targets)
            {
                if (target == null || target.transform == null)
                    continue;

                float currentDistance = Extensions.GetDistance(origin, target.transform);

                if (currentDistance >= minDistance)
                    continue;

                minDistance = currentDistance;
                nearestTarget = target;
            }

            return nearestTarget;
        }

        private static void ExcludeSelf(Object excludeCollider, IList<Collider> colliders)
        {
            if (excludeCollider == null)
                return;

            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i] == null)
                    continue;
                if (colliders[i] != excludeCollider)
                    continue;
                colliders[i] = null;
                return;
            }
        }
    }
}