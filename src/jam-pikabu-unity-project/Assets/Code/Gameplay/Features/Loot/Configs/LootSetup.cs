using System;
using Code.Gameplay.Features.Currency.Config;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features.Loot.Configs
{
    [Serializable]
    public class LootSetup
    {
        public LootTypeId Type;
        public EntityView ViewPrefab;
        public CostSetup GoldForPicking = new(CurrencyTypeId.Gold);
    }
}