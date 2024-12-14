using System;
using Code.Gameplay.Features.Loot;

namespace Code.Gameplay.Features.Orders.Config
{
    [Serializable]
    public struct IngredientData
    {
        public LootTypeId TypeId;
        public IngredientTypeId IngredientType;
        public int RatingFactor;
        public int Amount;

        public IngredientData(LootTypeId typeId, IngredientTypeId ingredientType, int ratingFactor, int amount)
        {
            TypeId = typeId;
            IngredientType = ingredientType;
            RatingFactor = ratingFactor;
            Amount = amount;
        }
    }
}