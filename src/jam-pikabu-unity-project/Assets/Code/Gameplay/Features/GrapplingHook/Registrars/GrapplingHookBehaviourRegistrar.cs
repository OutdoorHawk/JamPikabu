using Code.Gameplay.Features.GrapplingHook.Behaviours;
using Code.Infrastructure.View.Registrars;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Registrars
{
    public class GrapplingHookBehaviourRegistrar : EntityComponentRegistrar
    {
        [SerializeField] private GrapplingHookBehaviour _behaviour;

        public override void RegisterComponents()
        {
            Entity.AddGrapplingHookBehaviour(_behaviour);
        }

        public override void UnregisterComponents()
        {
            if (Entity.hasGrapplingHookBehaviour)
                Entity.RemoveGrapplingHookBehaviour();
        }
    }
}