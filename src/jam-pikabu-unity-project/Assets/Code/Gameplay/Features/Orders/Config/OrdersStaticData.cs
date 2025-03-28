﻿using System.Collections.Generic;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Gameplay.Features.Orders.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(OrdersStaticData), fileName = "Orders")]
    public class OrdersStaticData : BaseStaticData<OrderData>
    {
        public float BadIngredientGoldPenaltyFactor = 0.5f;
        public float BadIngredientRatingPenaltyFactor = 0.05f;
        public float OrderCompletedRatingBonusFactor = 0.15f;
        public float PerfectOrderRatingBonusFactor = 0.25f;

        public List<OrderIconData> IconsPool;
        
        public override void OnConfigInit()
        {
            base.OnConfigInit();

            AddNonUniqueIndex(data => (int)data.Setup.Tag);
        }

        public List<OrderData> GetOrdersByTag(OrderTag tag)
        {
            return GetNonUniqueByKey((int)tag);
        }
    }
}