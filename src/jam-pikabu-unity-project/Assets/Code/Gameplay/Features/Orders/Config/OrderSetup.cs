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
        public OrderTag Tag;
        public Sprite OverrideIcon;
        public CostSetup GoldReward = new(CurrencyTypeId.Gold);

        [MinMaxSlider(0, 99, true)] public Vector2Int DaysRange;
        [Tooltip("Необходимое кол-во продуктов для типа"), MinMaxSlider(1, 10, true)] public Vector2Int MinMaxNeedAmount = Vector2Int.one;
        [Tooltip("Кол-во типов хороших продуктов"), MinMaxSlider(1, 4, true)] public Vector2Int MinMaxGoodIngredients = Vector2Int.one;
        [Tooltip("Кол-во типов плохих продуктов"), MinMaxSlider(1, 2, true)] public Vector2Int MinMaxBadIngredients = Vector2Int.one;
        [Tooltip("Бонус рейтинга за продукт"), MinMaxSlider(1, 2, true)] public Vector2Int MinMaxIngredientsRatingFactor = Vector2Int.one;

        [ReadOnly, ShowInInspector] private int TotalIngredientsCount => MinMaxGoodIngredients.y * MinMaxNeedAmount.y;

        public Sprite OrderIcon { get; set; }

        private bool IsBoss => Tag.HasFlag(OrderTag.Boss);
    }
}