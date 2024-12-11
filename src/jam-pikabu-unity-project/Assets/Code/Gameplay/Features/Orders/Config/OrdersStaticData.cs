using Code.Gameplay.StaticData;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Gameplay.Features.Orders.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(OrdersStaticData), fileName = "Orders")]
    public class OrdersStaticData : BaseStaticData<OrderData>
    {
       
    }
}