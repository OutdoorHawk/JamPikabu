using System;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData.Data;
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
        
        public CostSetup GoldReward = new(CurrencyTypeId.Gold);
        public Vector2Int MinMaxDayToUnlock;
        
        public Vector2Int MinMaxAmount = Vector2Int.one;
        
        public Vector2Int MinMaxGoodIngredients = Vector2Int.one;
        public Vector2Int MinMaxBadIngredients;
        public Vector2Int MinMaxIngredientsRatingFactor  = Vector2Int.one;
    }
}