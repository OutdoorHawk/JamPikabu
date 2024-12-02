using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.Loot
{
    [Game] public sealed class Loot : IComponent { }
    [Game] public sealed class LootSpawner : IComponent { }
    [Game] public sealed class LootEffectsApplier : IComponent { }
    [Game] public sealed class LootTypeIdComponent : IComponent { public LootTypeId Value; }
    [Game] public sealed class GoldValue : IComponent { public CostSetup Value; }
    [Game] public sealed class LootItemUIComponent : IComponent { public LootItemUI Value; }
    [Game] public sealed class LootPickupProcessing : IComponent { }
    [Game] public sealed class Collected : IComponent { }
    [Game] public sealed class ReadyToApply : IComponent { }
    [Game] public sealed class ReadyToApplyEffects : IComponent { }
    [Game] public sealed class ReadyToApplyValues : IComponent { }
    [Game] public sealed class ApplyingEffects : IComponent { }
    [Game] public sealed class ApplyingValues : IComponent { }
    [Game] public sealed class Applied : IComponent { }
}
