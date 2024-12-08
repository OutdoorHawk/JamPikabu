using Code.Gameplay.Features.Orders.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Orders
{
    public sealed class OrderCompletionFeature : Feature
    {
        public OrderCompletionFeature(ISystemFactory systems)
        {
            Add(systems.Create<CompleteOrderOnRoundCompletionSystem>());

            Add(systems.Create<PlayOrderWindowOnLootConsumedVisualsSystem>());
        }
    }
}