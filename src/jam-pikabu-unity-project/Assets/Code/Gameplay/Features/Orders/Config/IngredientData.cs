using System;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot;

namespace Code.Gameplay.Features.Orders.Config
{
    [Serializable]
    public struct IngredientData
    {
        public LootTypeId TypeId;
        public CostSetup Rating;
        //public int NeedCount;
    }
}