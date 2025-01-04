using Code.Gameplay.Features.Abilities.Behaviours;
using Code.Infrastructure.View.Registrars;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Registrars
{
    public class AbilityVisualsRegistrar : EntityComponentRegistrar
    {
        [SerializeField] private AbilityVisuals _abilityVisuals;

        public override void RegisterComponents()
        {
            Entity.AddAbilityVisuals(_abilityVisuals);
        }

        public override void UnregisterComponents()
        {
            if (Entity.hasAbilityVisuals)
                Entity.RemoveAbilityVisuals();
        }
    }
}