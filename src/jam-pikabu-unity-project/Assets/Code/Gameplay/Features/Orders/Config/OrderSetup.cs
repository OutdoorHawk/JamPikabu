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
        public CostSetup Reward = new(CurrencyTypeId.Gold);
        public Vector2Int MinMaxDayToUnlock;
        public bool IsBoss;

        public int GoodMinimum;
        public int BadMaximum;

        public bool RandomSetupEnabled = true;
        public Vector2Int MinMaxGoodIngredients = Vector2Int.one;
        public Vector2Int MinMaxGoodIngredientsReward = Vector2Int.one;
        public Vector2Int MinMaxBadIngredients;
        public Vector2Int MinMaxBadIngredientsReward;

        /*[Header("ManualSetup")]
        public List<IngredientData> GoodIngredients;
        public List<IngredientData> BadIngredients;*/
        // public int UniqueIngredientsForBonus;
    }
}