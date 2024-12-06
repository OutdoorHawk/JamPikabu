using UnityEngine;

namespace Code.Gameplay.Features.Conveyor.Behaviours
{
    public class CollisionTeleport : MonoBehaviour
    {
        [SerializeField] private Transform teleportTo;

        private void OnCollisionEnter2D(Collision2D other)
        {
            other.rigidbody.position = teleportTo.position;
        }

        private void OnCollisionStay(Collision other)
        {
            other.rigidbody.position = teleportTo.position;
        }
    }
}