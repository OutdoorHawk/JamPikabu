using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Features.Orders.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(OrdersStaticData), fileName = "Orders")]
    public class OrdersStaticData : BaseStaticData<OrderData>
    {
        public int OrdersAmountInDay = 4;
    }
}