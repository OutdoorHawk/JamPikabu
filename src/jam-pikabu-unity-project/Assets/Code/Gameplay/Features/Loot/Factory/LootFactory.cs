using System;
using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Factory
{
    public class LootFactory : ILootFactory
    {
        private readonly IStaticDataService _staticDataService;

        public LootFactory(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public GameEntity CreateLootSpawner()
        {
            GameEntity lootSpawner = CreateGameEntity.Empty()
                .With(x => x.isLootSpawner = true);
            return lootSpawner;
        }

        public GameEntity CreateLootEntity(LootTypeId typeId, Transform parent, Vector2 at, Vector3 spawnRotation)
        {
            LootSetup lootSetup = GetLootSetup(typeId);
            GameEntity loot = CreateBaseLoot(typeId, parent, at, spawnRotation);

            AddEffects(loot, typeId);
            
            switch (typeId)
            {
                case LootTypeId.Unknown:
                    break;
            }

            return loot;
        }

        private GameEntity CreateBaseLoot(LootTypeId typeId, Transform parent, Vector2 at, Vector3 spawnRotation)
        {
            LootSetup lootSetup = GetLootSetup(typeId);

            GameEntity loot = CreateGameEntity.Empty()
                    .With(x => x.isLoot = true)
                    .AddStartWorldPosition(at)
                    .AddStartRotation(spawnRotation)
                    .AddTargetParent(parent)
                    .AddLootTypeId(typeId)
                    .AddViewPrefab(lootSetup.ViewPrefab)
                ;

            return loot;
        }

        private LootSetup GetLootSetup(LootTypeId typeId)
        {
            var staticData = _staticDataService.GetStaticData<LootStaticData>();
            var lootSetup = staticData.GetConfig(typeId);
            return lootSetup;
        }

        private void AddEffects(GameEntity loot, LootTypeId typeId)
        {
            LootSetup lootSetup = GetLootSetup(typeId);
        }
    }
}