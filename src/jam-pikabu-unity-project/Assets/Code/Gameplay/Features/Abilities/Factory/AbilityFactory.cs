using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities.Config;
using Code.Gameplay.StaticData;

namespace Code.Gameplay.Features.Abilities.Factory
{
    public class AbilityFactory : IAbilityFactory
    {
        private readonly IStaticDataService _staticData;

        private AbilityStaticData AbilityStaticData => _staticData.Get<AbilityStaticData>();

        public AbilityFactory(IStaticDataService staticData)
        {
            _staticData = staticData;
        }

        public GameEntity CreateAbility(AbilityTypeId typeId, int targetId)
        {
            if (typeId == AbilityTypeId.None)
                return null;
            
            GameEntity ability = CreateBaseAbility(typeId, targetId);

            switch (typeId)
            {
                case AbilityTypeId.Bouncy:
                    CreateBouncyAbility(ability, typeId);
                    break;
            }

            return ability;
        }

        private GameEntity CreateBaseAbility(AbilityTypeId typeId, int targetId)
        {
            var ability = CreateGameEntity
                    .Empty()
                    .With(x => x.isAbility = true)
                    .AddAbilityType(typeId)
                    .AddTarget(targetId)
                ;
            
            return ability;
        }

        private void CreateBouncyAbility(GameEntity ability, AbilityTypeId typeId)
        {
            AbilityData staticData = AbilityStaticData.GetDataByType(typeId);

            ability
                .With(x => x.isBouncyAbility = true)
                .AddBounceStrength(staticData.Value)
                .AddCooldown(staticData.Cooldown)
                .AddCooldownLeft(staticData.Cooldown)
                ;
        }
    }
}