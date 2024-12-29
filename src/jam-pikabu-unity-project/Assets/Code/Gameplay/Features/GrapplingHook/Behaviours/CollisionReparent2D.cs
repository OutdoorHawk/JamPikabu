using System.Collections.Generic;
using Code.Common.Extensions;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Behaviours
{
    public class CollisionReparent2D : MonoBehaviour
    {
        // Храним исходных родителей для каждого объекта, который мы «подхватили»
        private readonly Dictionary<Transform, Transform> originalParents = new();

        [Header("Настройка слоя")] [Tooltip("Объекты с этим слоем будут репарентиться при столкновении.")] [SerializeField]
        private LayerMask targetLayer = 0; // Укажите тут номер слоя (например, 8)

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Проверяем, принадлежит ли объект нужному слою
            if (collision.gameObject.layer.Matches(targetLayer) == false)
            {
                // Если слой не совпадает, ничего не делаем
                return;
            }

            Transform otherTransform = collision.transform;

            if (originalParents.ContainsKey(otherTransform))
                return;

            originalParents[otherTransform] = otherTransform.parent;
            otherTransform.SetParent(transform);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Matches(targetLayer) == false)
                return;

            Transform otherTransform = collision.transform;
            
            if (originalParents.ContainsKey(otherTransform))
            {
                otherTransform.SetParent(originalParents[otherTransform]);
                originalParents.Remove(otherTransform);
            }
        }
    }
}