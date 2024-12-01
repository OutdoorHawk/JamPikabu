using UnityEngine;

namespace Code.Gameplay.Features.Loot.Factory
{
    public interface ILootFactory
    {
        GameEntity CreateLootEntity(LootTypeId typeId, Transform parent, Vector2 spawnPosition);
    }
}