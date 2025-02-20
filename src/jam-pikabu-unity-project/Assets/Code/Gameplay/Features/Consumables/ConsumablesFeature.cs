using Code.Gameplay.Features.Consumables.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Consumables
{
    public sealed class ConsumablesFeature : Feature
    {
        public ConsumablesFeature(ISystemFactory systems)
        {
            Add(systems.Create<ProcessConsumableInRoundLoopSystem>());
            Add(systems.Create<ActivateSpoonConsumableSystem>());
            Add(systems.Create<ActivateWoodConsumableSystem>());
            Add(systems.Create<ProcessConsumableSpendSystem>());
            Add(systems.Create<ClearAnyActivateConsumablesRequests>());
        }
    }
}