using System;
using Code.Gameplay.Features.Loot;

namespace Code.Gameplay.Features.Orders.Config
{
    [Serializable]
    public struct IngredientData
    {
        public LootTypeId TypeId;
        public int RatingForAdd;
        //public int NeedCount;
    }
}