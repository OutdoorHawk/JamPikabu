using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.Loot
{
    [Game] public sealed class Loot : IComponent { }
    [Game] public sealed class LootSpawner : IComponent { }
    [Game] public sealed class LootTypeIdComponent : IComponent { public LootTypeId Value; }
    [Game] public sealed class GoldForPickUp : IComponent { public CostSetup Value; }
    [Game] public sealed class LootItemUIComponent : IComponent { public LootItemUI Value; }
    [Game] public sealed class LootPickupProcessing : IComponent { }
}
