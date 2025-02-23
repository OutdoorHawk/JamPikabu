using System;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Code.Meta.Features.Consumables;
using UnityEngine;

namespace Code.Gameplay.Features.Consumables.Config
{
    [Serializable]
    public class ConsumablesData : BaseData
    {
        public ConsumableTypeId TypeId;
        public LootTypeId LootTypeId;
        public float CooldownSeconds;
        public int Value;
        public int LevelNeedToUnlockSpawn;
        [Range(0, 101)] public int SpawnChanceInOrder;
    }
}