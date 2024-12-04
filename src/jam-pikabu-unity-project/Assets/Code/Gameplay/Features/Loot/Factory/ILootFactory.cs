using UnityEngine;

namespace Code.Gameplay.Features.Loot.Factory
{
    public interface ILootFactory
    {
        GameEntity CreateLootSpawner();
        GameEntity CreateLootEntity(LootTypeId typeId, Transform parent, Vector2 at, Vector3 spawnRotation);
    }
}