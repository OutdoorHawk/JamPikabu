using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Infrastructure.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [Serializable]
    public class LootSetup
    {
        public LootTypeId Type;
        public EntityView ViewPrefab;
        public Sprite Icon;
        [HideLabel] public CostSetup Value = new(CurrencyTypeId.Gold);
        [Header("Effects")] public float EffectValue;
        public List<LootTypeId> EffectTargets;
    }
}