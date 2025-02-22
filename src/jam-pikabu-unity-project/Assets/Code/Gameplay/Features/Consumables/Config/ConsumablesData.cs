using System;
using Code.Gameplay.StaticData.Data;
using Code.Meta.Features.Consumables;

namespace Code.Gameplay.Features.Consumables.Config
{
    [Serializable]
    public class ConsumablesData : BaseData
    {
        public ConsumableTypeId TypeId;
        public float CooldownSeconds;
        public int Value;
    }
}