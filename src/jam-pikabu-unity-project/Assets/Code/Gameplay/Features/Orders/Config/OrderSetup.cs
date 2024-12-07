using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using Sirenix.OdinInspector;
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
        [PreviewField] public Sprite OrderIcon;
        public CostSetup Reward = new(CurrencyTypeId.Gold);
        public int MinDayToUnlock;
        public int MaxDayToUnlock;
       
        public List<IngredientData> GoodIngredients;
        public List<IngredientData> BadIngredients;
        /*[Header("RandomDish")]
        public bool RandomIngredients;
        public int MinGoodIngredients;
        public int MaxGoodIngredients;*/
        // public int UniqueIngredientsForBonus;
    }
}