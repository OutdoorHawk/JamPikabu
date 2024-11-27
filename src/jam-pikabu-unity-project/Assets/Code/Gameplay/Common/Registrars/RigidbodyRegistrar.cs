using Code.Infrastructure.View.Registrars;
using UnityEngine;

namespace Code.Gameplay.Common.Registrars
{
    public class RigidbodyRegistrar : EntityComponentRegistrar
    {
        public override void RegisterComponents()
        {
            Entity.AddRigidbody(GetComponent<Rigidbody>());
        }

        public override void UnregisterComponents()
        {
            if (Entity.hasRigidbody)
                Entity.RemoveRigidbody();
        }
    }
}