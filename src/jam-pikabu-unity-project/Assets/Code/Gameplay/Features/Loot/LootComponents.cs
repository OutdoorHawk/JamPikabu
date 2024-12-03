using System.Collections.Generic;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.Loot
{
    [Game] public sealed class Loot : IComponent { }
    [Game] public sealed class LootSpawner : IComponent { }
    [Game] public sealed class LootEffectsApplier : IComponent { }
    [Game] public sealed class LootTypeIdComponent : IComponent { public LootTypeId Value; }
    [Game] public sealed class GoldValue : IComponent { public int Value; }
    [Game] public sealed class LootItemUIComponent : IComponent { public LootItemUI Value; }
    [Game] public sealed class LootPickupProcessing : IComponent { }
    [Game] public sealed class Collected : IComponent { }
    [Game] public sealed class ReadyToApply : IComponent { }
    [Game] public sealed class ReadyToApplyEffects : IComponent { }
    [Game] public sealed class ReadyToApplyValues : IComponent { }
    [Game] public sealed class ApplyingEffects : IComponent { }
    [Game] public sealed class EffectTargetsLoot : IComponent { public List<LootTypeId> Value; }
    [Game] public sealed class ApplyingValues : IComponent { }
    [Game] public sealed class Applied : IComponent { }
    [Game] public sealed class EffectApplied : IComponent { }
    [Game] public sealed class EffectApplicationAvailable : IComponent { }
    [Game] public sealed class LootConsumeAvailable : IComponent { }
    [Game] public sealed class Consumed : IComponent { }
    [Game] public sealed class EffectValue : IComponent { public float Value; }
}
