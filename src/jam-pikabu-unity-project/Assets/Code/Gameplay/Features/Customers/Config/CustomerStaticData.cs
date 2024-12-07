using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Features.Customers.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(CustomerStaticData), fileName = "Customer")]
    public class CustomerStaticData : BaseStaticData<CustomerSetup>
    {
        
    }
}