using Code.Gameplay.Features.RoundState.Behaviours;
using Code.Infrastructure.View.Registrars;
using UnityEngine;

namespace Code.Gameplay.Features.RoundState.Registrars
{
    public class RoundStateViewBehaviourRegistrar : EntityComponentRegistrar
    {
        [SerializeField] private RoundStateViewBehaviour _roundStateViewBehaviour;

        public override void RegisterComponents()
        {
            Entity.AddRoundStateViewBehaviour(_roundStateViewBehaviour);
        }

        public override void UnregisterComponents()
        {
            if (Entity.hasRoundStateViewBehaviour)
                Entity.RemoveRoundStateViewBehaviour();
        }
    }
}