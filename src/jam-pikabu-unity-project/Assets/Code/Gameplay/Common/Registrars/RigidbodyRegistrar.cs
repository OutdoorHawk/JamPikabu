using Code.Infrastructure.View.Registrars;
using UnityEngine;

namespace Code.Gameplay.Common.Registrars
{
    public class RigidbodyRegistrar : EntityComponentRegistrar
    {
        public override void RegisterComponents()
        {
            /*
            if (TryGetComponent(out Rigidbody rigidbody))
                Entity.AddRigidbody(rigidbody);
                */
            
            if (TryGetComponent(out Rigidbody2D rigidbody2d))
                Entity.AddRigidbody2D(rigidbody2d);
           
        }

        public override void UnregisterComponents()
        {
            /*
            if (Entity.hasRigidbody)
                Entity.RemoveRigidbody();
                */
            
            if (Entity.hasRigidbody2D)
                Entity.RemoveRigidbody2D();
        }
    }
}