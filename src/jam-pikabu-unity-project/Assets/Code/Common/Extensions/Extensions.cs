using DG.Tweening;
using UnityEngine;

namespace Code.Common.Extensions
{
    public static class Extensions
    {
        public const float FADE_DURATION = 0.25F;

        public const Ease STOPPING_EASE = Ease.OutBounce;

        public const float STOPPING_ROTATION_RATE = 0.25f;
        
        public static bool IsLayerInLayerMask(LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) > 0;
        }

        public static int LayerMaskToLayer(LayerMask layerMask)
        {
            for (int i = 0; i < 32; i++)
            {
                int layer = 1 << i;
                if ((layerMask & layer) != 0)
                    return i;
            }

            Debug.LogWarning("Invalid layer index!");
            return -1;
        }


        public static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public static void ClearArray<T>(T[] array) where T : class
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = null;
        }

        public static float GetDistance(Transform origin, Transform target)
        {
            Vector3 originPosition = origin.position;
            Vector3 targetPosition = target.position;
            return Vector3.Distance(originPosition, targetPosition);
        }
        
        public static float GetHorizontalDistance(Transform origin, Transform target)
        {
            Vector3 originPosition = origin.position;
            Vector3 targetPosition = target.position;
            originPosition.y = 0;
            targetPosition.y = 0;
            return Vector3.Distance(originPosition, targetPosition);
        }

        public static float GetCurrentAnimationLength(Animator animator)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            return animState.length;
        }
        
        public static Vector3 CalculateTargetRotation(Vector3 directionToTarget)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized);
            Vector3 nextRotationEuler = targetRotation.eulerAngles;
            nextRotationEuler = new Vector3(nextRotationEuler.x, nextRotationEuler.y, nextRotationEuler.z);
            return nextRotationEuler;
        }
    }
}