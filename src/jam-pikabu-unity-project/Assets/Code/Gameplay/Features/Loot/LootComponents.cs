using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.Loot
{
    [Game] public sealed class Loot : IComponent { }
    [Game] public sealed class LootSpawner : IComponent { }
    [Game] public sealed class SingleSpawn : IComponent { }
    [Game] public sealed class ContinuousSpawn : IComponent { }
    [Game] public sealed class ConveyorSpawner : IComponent { }
    [Game] public sealed class LootEffectsApplier : IComponent { }
    [Game] public sealed class LootTypeIdComponent : IComponent { public LootTypeId Value; }
    [Game] public sealed class BaseRating : IComponent { public int Value; }
    [Game] public sealed class Rating : IComponent { public int Value; }
    [Game] public sealed class LootItemUIComponent : IComponent { public LootItemUI Value; }
    [Game] public sealed class LootItemComponent : IComponent { public LootItem Value; }
    [Game] public sealed class Collected : IComponent { }
    
    [Game] public sealed class EffectTargetsLoot : IComponent { public List<LootTypeId> Value; }
    [Game] public sealed class Applied : IComponent { }
    [Game] public sealed class Consumed : IComponent { }
    [Game] public sealed class EffectValue : IComponent { public float Value; }
    [Game] public sealed class IncreaseValueEffect : IComponent { }
    [Game] public sealed class Available : IComponent { }
    [Game] public sealed class Effect : IComponent { }
}
