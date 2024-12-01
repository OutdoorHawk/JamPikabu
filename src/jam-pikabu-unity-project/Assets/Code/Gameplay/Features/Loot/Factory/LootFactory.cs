using System;
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
        
        public GameEntity CreateLootEntity(LootTypeId typeId, Transform parent, Vector2 at, Vector3 spawnRotation)
        {
            var staticData = _staticDataService.GetStaticData<LootStaticData>();
            var lootSetup = staticData.GetConfig(typeId);

            GameEntity loot = CreateGameEntity.Empty()
                    .With(x => x.isLoot = true)
                    .AddStartWorldPosition(at)
                    .AddStartRotation(spawnRotation)
                    .AddTargetParent(parent)
                    .AddLootTypeId(typeId)
                    .AddViewPrefab(lootSetup.ViewPrefab)
                ;

            switch (typeId)
            {
                case LootTypeId.Unknown:
                    break;
                case LootTypeId.GoldCoin:
                    break;
                case LootTypeId.Toy:
                    break;
                case LootTypeId.Trash:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeId), typeId, null);
            }

            return loot;
        }
    }
}