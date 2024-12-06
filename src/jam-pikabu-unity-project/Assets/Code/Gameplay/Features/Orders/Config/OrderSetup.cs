using System;
using System.Collections.Generic;
using Code.Gameplay.StaticData;

namespace Code.Gameplay.Features.Orders.Config
{
    [Serializable]
    public class OrderData : BaseData
    {
        public OrderSetup Setup;
    }

    [Serializable]
    public class OrderSetup
    {
        public int MinDayToUnlock;
        public int MaxDayToUnlock;
        public List<IngredientData> GoodIngredients;
        public List<IngredientData> BadIngredients;
        public int UniqueIngredientsForBonus;
    }
}