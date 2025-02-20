using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Meta.Features.Consumables;

namespace Code.Gameplay.Features.Consumables.Factory
{
    public class ConsumablesFactory : IConsumablesFactory
    {
        public void ActivateConsumable(ConsumableTypeId type)
        {
            GameEntity activateRequest = CreateGameEntity.Empty()
                .With(x => x.isActivateConsumableRequest = true)
                .AddConsumableTypeId(type);

            switch (type)
            {
                case ConsumableTypeId.Wood:
                    activateRequest
                        .With(x => x.isWood = true);
                    break;
                case ConsumableTypeId.Spoon:
                    activateRequest
                        .With(x => x.isSpoon = true);
                    break;
            }
        }
    }
}