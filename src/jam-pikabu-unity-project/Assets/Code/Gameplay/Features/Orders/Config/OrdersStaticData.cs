using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Gameplay.Features.Orders.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(OrdersStaticData), fileName = "Orders")]
    public class OrdersStaticData : BaseStaticData<OrderData>
    {
        public float BadIngredientPenalty = 0.5f;
    }
}