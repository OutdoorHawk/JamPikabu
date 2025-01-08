using Code.Gameplay.Features.Distraction.Behaviours;
using Code.Infrastructure.View.Registrars;
using UnityEngine;

namespace Code.Gameplay.Features.Distraction.Registrars
{
    public class BeeBehaviourRegistrar : EntityComponentRegistrar
    {
        [SerializeField] private BeeBehaviour _beeBehaviour;

        public override void RegisterComponents()
        {
            Entity.AddBeeBehaviour(_beeBehaviour);
        }

        public override void UnregisterComponents()
        {
            if (Entity.hasBeeBehaviour)
                Entity.RemoveBeeBehaviour();
        }
    }
}