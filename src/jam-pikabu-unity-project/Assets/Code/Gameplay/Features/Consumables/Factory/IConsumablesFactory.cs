using Code.Meta.Features.Consumables;

namespace Code.Gameplay.Features.Consumables.Factory
{
    public interface IConsumablesFactory
    {
        void ActivateConsumable(ConsumableTypeId type);
    }
}