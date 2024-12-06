using Code.Gameplay.Features.Orders.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Orders
{
    public sealed class OrdersFeature : Feature
    {
        public OrdersFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitOrdersDaySystem>());
            Add(systems.Create<InitLootValueForOrderOnRoundStartSystem>());
        }
    }
}