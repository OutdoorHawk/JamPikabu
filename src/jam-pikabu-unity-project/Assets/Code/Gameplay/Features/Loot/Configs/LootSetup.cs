using System;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [Serializable]
    public class LootSetup
    {
        public LootTypeId Type;
        public EntityView ViewPrefab;
        public Sprite Icon;
        public CostSetup GoldForPicking = new(CurrencyTypeId.Gold);
    }
}