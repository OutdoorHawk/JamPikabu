using Code.Common.Extensions;
using UnityEngine;

namespace Code.Gameplay.Features.Conveyor.Behaviours
{
    public class ConveyorBelt : MonoBehaviour
    {
        [SerializeField] private Vector2 conveyorForce = new(1f, 0f);
        [SerializeField] private LayerMask targetLayer = 0;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer.Matches(targetLayer) == false)
                return;
            
            Rigidbody2D rb = other.attachedRigidbody;
            
            if (rb != null)
            {
                rb.AddForce(conveyorForce, ForceMode2D.Force);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.layer.Matches(targetLayer) == false)
                return;
            
            Rigidbody2D rb = other.rigidbody;
            
            if (rb != null)
            {
                rb.linearVelocity = conveyorForce;
            }
        }
    }
}