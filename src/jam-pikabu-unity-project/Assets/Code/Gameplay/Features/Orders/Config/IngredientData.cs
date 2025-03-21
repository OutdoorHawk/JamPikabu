﻿using System;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Loot;

namespace Code.Gameplay.Features.Orders.Config
{
    [Serializable]
    public struct IngredientData
    {
        public LootTypeId TypeId;
        public IngredientTypeId IngredientType;
        public CurrencyTypeId RatingType;
        public int RatingFactor;
        public int Amount;

        public IngredientData
        (
            LootTypeId typeId,
            IngredientTypeId ingredientType,
            int ratingFactor,
            int amount
        )
        {
            TypeId = typeId;
            IngredientType = ingredientType;
            RatingFactor = ratingFactor;
            Amount = amount;
            RatingType = ingredientType is IngredientTypeId.Good
                ? CurrencyTypeId.Plus
                : CurrencyTypeId.Minus;
        }
    }
}