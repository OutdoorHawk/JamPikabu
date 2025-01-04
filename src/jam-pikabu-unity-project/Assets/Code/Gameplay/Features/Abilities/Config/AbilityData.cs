using System;
using Code.Gameplay.StaticData.Data;

namespace Code.Gameplay.Features.Abilities.Config
{
    [Serializable]
    public class AbilityData : BaseData
    {
        public AbilityTypeId Type;
        public float Value;
        public float Cooldown;
    }
}