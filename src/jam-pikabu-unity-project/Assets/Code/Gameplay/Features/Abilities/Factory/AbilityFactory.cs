using System;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities.Config;
using Code.Gameplay.StaticData;
using Random = UnityEngine.Random;

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
                case AbilityTypeId.SwapPositions:
                    CreateSwapPositionsAbility(ability, typeId);
                    break;
                case AbilityTypeId.ChangeSizes:
                    CreateChangeSizesAbility(ability, typeId);
                    break;
                case AbilityTypeId.HookSpeedChange:
                    CreateHookSpeedChangeAbility(ability, typeId);
                    break;
                case AbilityTypeId.PickupRandomLoot:
                    CreatePickupRandomLootAbility(ability, typeId);
                    break;
                case AbilityTypeId.SinglePickup:
                    CreateSinglePickupAbility(ability, typeId);
                    break;
                case AbilityTypeId.MultiPickup:
                    CreateMultiPickupAbility(ability, typeId);
                    break;
                case AbilityTypeId.HeavyObject:
                    CreateHeavyObjectAbility(ability, typeId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeId), typeId, null);
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
                .AddCooldownLeft(Random.Range(1, staticData.Cooldown + 1))
                ;
        }

        private void CreateSwapPositionsAbility(GameEntity ability, AbilityTypeId typeId)
        {
            AbilityData staticData = AbilityStaticData.GetDataByType(typeId);

            ability
                .With(x => x.isSwapPositionsAbility = true)
                .AddCooldown(staticData.Cooldown)
                .AddCooldownLeft(Random.Range(1, staticData.Cooldown + 1))
                ;
        }

        private void CreateChangeSizesAbility(GameEntity ability, AbilityTypeId typeId)
        {
            AbilityData staticData = AbilityStaticData.GetDataByType(typeId);

            ability
                .With(x => x.isChangeSizesAbility = true)
                .AddCooldown(staticData.Cooldown)
                .AddCooldownLeft(Random.Range(1, staticData.Cooldown + 1))
                ;
        }

        private void CreateHookSpeedChangeAbility(GameEntity ability, AbilityTypeId typeId)
        {
            AbilityData staticData = AbilityStaticData.GetDataByType(typeId);

            ability
                .With(x => x.isHookSpeedChangeAbility = true)
                .AddAbilityDuration(staticData.Cooldown)
                .AddSpeedChangeAmount(staticData.Value)
                .AddActivationChance(staticData.ActivationChance)
                ;
        }

        private void CreatePickupRandomLootAbility(GameEntity ability, AbilityTypeId typeId)
        {
            ability
                .With(x => x.isPickupRandomLootAbility = true)
                ;
        }

        private void CreateSinglePickupAbility(GameEntity ability, AbilityTypeId typeId)
        {
            ability
                .With(x => x.isSinglePickupAbility = true)
                ;
        }

        private void CreateMultiPickupAbility(GameEntity ability, AbilityTypeId typeId)
        {
            ability
                .With(x => x.isMultiPickupAbility = true)
                ;
        }

        private void CreateHeavyObjectAbility(GameEntity ability, AbilityTypeId typeId)
        {
            AbilityData staticData = AbilityStaticData.GetDataByType(typeId);

            ability
                .With(x => x.isHeavyObjectAbility = true)
                .AddHeavyObjectSpeedFactor(staticData.Value)
                ;
        }
    }
}