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

        [Tooltip("Необходимое кол-во продуктов для типа")] public Vector2Int MinMaxNeedAmount = Vector2Int.one;
        [Tooltip("Кол-во типов хороших продуктов")] public Vector2Int MinMaxGoodIngredients = Vector2Int.one;
        [Tooltip("Кол-во типов плохих продуктов")] public Vector2Int MinMaxBadIngredients;
        [Tooltip("Бонус рейтинга за продукт")] public Vector2Int MinMaxIngredientsRatingFactor = Vector2Int.one;
    }
}