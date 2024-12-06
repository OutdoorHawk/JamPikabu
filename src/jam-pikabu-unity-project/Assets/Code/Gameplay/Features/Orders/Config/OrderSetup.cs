using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using UnityEngine;

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
        public Sprite OrderIcon;
        public CostSetup DefaultCost = new(CurrencyTypeId.Gold);
        public int MinDayToUnlock;
        public int MaxDayToUnlock;
        public List<IngredientData> GoodIngredients;
        public List<IngredientData> BadIngredients;
        public int UniqueIngredientsForBonus;
    }
}