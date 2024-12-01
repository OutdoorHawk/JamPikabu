using System;
using Code.Common.Entity;
using Code.Common.Extensions;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Factory
{
    public class LootFactory : ILootFactory
    {
        public GameEntity CreateLootEntity(LootTypeId typeId, Transform parent, Vector2 spawnPosition)
        {
            GameEntity loot = CreateGameEntity.Empty()
                    .With(x => x.isLoot = true)
                    .AddStartWorldPosition(spawnPosition)
                    .AddTargetParent(parent)
                // .AddLootTypeId(typeId)
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