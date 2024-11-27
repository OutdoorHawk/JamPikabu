using UnityEngine;

namespace Code.Common.Extensions
{
    public static class GameObjectExtensions
    {
        public static void EnableElement(this GameObject element)
        {
            if (element.activeSelf == false)
                element.SetActive(true);
        }

        public static void DisableElement(this GameObject element)
        {
            if (element.activeSelf)
                element.SetActive(false);
        }

        public static void EnableElement(this Component element)
        {
            if (element.gameObject.activeSelf == false)
                element.gameObject.SetActive(true);
        }

        public static void DisableElement(this Component element)
        {
            if (element.gameObject.activeSelf)
                element.gameObject.SetActive(false);
        }

        public static void EnableSafe(this GameObject element)
        {
            if (element == null)
                return;

            element.EnableElement();
        }
        
        public static void EnableSafe(this Component element)
        {
            if (element == null)
                return;

            element.EnableElement();
        }
        
        public static void DisableSafe(this Component element)
        {
            if (element == null)
                return;

            element.DisableElement();
        }
        
        public static void DisableSafe(this GameObject element)
        {
            if (element == null)
                return;

            element.DisableElement();
        }

        public static void SetBehaviorEnabled(this Behaviour component)
        {
            if (component.enabled)
                return;
            
            component.enabled = true;
        }
        
        public static void SetBehaviorDisabled(this Behaviour component)
        {
            if (component.enabled == false)
                return;
            
            component.enabled = false;
        }
        
        public static void SetBehaviorEnabledSafe(this Behaviour component)
        {
            if (component == null)
                return;
            
            if (component.enabled)
                return;
            
            component.enabled = true;
        }
        
        public static void SetBehaviorDisabledSafe(this Behaviour component)
        {
            if (component == null)
                return;
            
            if (component.enabled == false)
                return;
            
            component.enabled = false;
        }

        public static bool IsNullOrDestroyed(this MonoBehaviour behaviour)
        {
            if (behaviour == null)
                return true;

            if (behaviour.destroyCancellationToken.IsCancellationRequested)
                return true;

            return false;
        }
    }
}