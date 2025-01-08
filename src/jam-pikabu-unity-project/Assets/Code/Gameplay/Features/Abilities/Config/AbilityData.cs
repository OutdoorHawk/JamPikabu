using System;
using Code.Gameplay.StaticData.Data;
using UnityEngine.Localization;

namespace Code.Gameplay.Features.Abilities.Config
{
    [Serializable]
    public class AbilityData : BaseData
    {
        public AbilityTypeId Type;
        public float Value;
        public float Cooldown;
        public int ActivationChance = 100;
        public LocalizedString LocalizedDescription;
    }
}