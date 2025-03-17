using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities.Factory;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.ABTesting;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Service;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Factory
{
    public class LootFactory : ILootFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IAbilityFactory _abilityFactory;
        private readonly IABTestService _abTestService;

        public LootFactory
        (
            IStaticDataService staticDataService,
            IDaysService daysService,
            ILootCollectionService lootCollectionService,
            IAbilityFactory abilityFactory,
            IABTestService abTestService
        )
        {
            _abTestService = abTestService;
            _staticDataService = staticDataService;
            _daysService = daysService;
            _lootCollectionService = lootCollectionService;
            _abilityFactory = abilityFactory;
        }

        public GameEntity CreateLootEntity(LootTypeId typeId, Transform parent, Vector2 at, Vector3 spawnRotation)
        {
            GameEntity loot = CreateBaseLoot(typeId, parent, at, spawnRotation);
            LootSettingsData lootSetup = GetLootSetup(loot.LootTypeId);

            loot.With(x => AddRating(x, typeId), when: lootSetup.CanBeUsedInOrders);

            switch (typeId)
            {
                case LootTypeId.GoldCoin:
                    CreateGold(loot);
                    break;
                case LootTypeId.Wood:
                    CreateWood(loot);
                    break;
                case LootTypeId.WoodChip:
                    CreateWoodChip(loot);
                    break;
                case LootTypeId.Spoon:
                    CreateSpoon(loot);
                    break;
            }

            _abilityFactory.CreateAbility(lootSetup.AbilityType, loot.Id);
            return loot;
        }

        private void CreateGold(GameEntity loot)
        {
            loot.AddGold(Mathf.CeilToInt(1 * _daysService.GetDayGoldFactor()));
        }

        private void CreateWood(GameEntity loot)
        {
            LootSettingsData lootSetup = GetLootSetup(loot.LootTypeId);

            loot
                .With(x => x.isWood = true)
                .AddConsumableTypeId(ConsumableTypeId.Wood)
                ;
        }
        
        private void CreateWoodChip(GameEntity loot)
        {
            LootSettingsData lootSetup = GetLootSetup(loot.LootTypeId);

            int effectValue = (int)lootSetup.EffectValue;

            if (_abTestService.GetExperimentValue(ExperimentTagTypeId.TIMER_REPLACE) is ExperimentValueTypeId.replace_timer_with_attempts) 
                effectValue = 1;
            
            loot
                .With(x => x.isWoodChip = true)
                .AddTimerRefillAmount(effectValue)
                ;
        }

        private void CreateSpoon(GameEntity loot)
        {
            LootSettingsData lootSetup = GetLootSetup(loot.LootTypeId);

            loot
                .With(x => x.isSpoon = true)
                .AddConsumableTypeId(ConsumableTypeId.Spoon)
                ;
        }

        private GameEntity CreateBaseLoot(LootTypeId typeId, Transform parent, Vector2 at, Vector3 spawnRotation)
        {
            LootSettingsData lootSetup = GetLootSetup(typeId);
            var staticData = _staticDataService.Get<LootSettingsStaticData>();

            GameEntity loot = CreateGameEntity.Empty()
                    .With(x => x.isLoot = true)
                    .AddStartWorldPosition(at)
                    .AddStartRotation(spawnRotation)
                    .AddTargetParent(parent)
                    .AddLootTypeId(typeId)
                    .With(x => x.AddViewPrefab(staticData.LootItem), when: lootSetup.ViewPrefab == null)
                    .With(x => x.AddViewPrefab(lootSetup.ViewPrefab), when: lootSetup.ViewPrefab != null)
                ;

            return loot;
        }

        private void AddRating(GameEntity loot, LootTypeId typeId)
        {
            LootSettingsData lootSetup = GetLootSetup(typeId);

            loot.AddBaseRating(lootSetup.BaseRatingValue)
                .With(x => x.isConsumableIngredient = true);

            loot.AddRating(loot.BaseRating);

            if (_lootCollectionService.TryGetLootLevel(typeId, out var levelData))
            {
                loot.ReplaceRating(loot.BaseRating + levelData.RatingBoostAmount);
            }
        }

        private LootSettingsData GetLootSetup(LootTypeId typeId)
        {
            var staticData = _staticDataService.Get<LootSettingsStaticData>();
            var lootSetup = staticData.GetConfig(typeId);
            return lootSetup;
        }
    }
}