using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Entitas;

namespace Code.Gameplay.Features.LootSpawning
{
    [Game] public sealed class LootSpawner : IComponent { }
    [Game] public sealed class OneTimeSpawner : IComponent { }
    [Game] public sealed class BonusLevelSpawn : IComponent { }
    [Game] public sealed class ContinuousSpawn : IComponent { }
    [Game] public sealed class ConveyorSpawner : IComponent { }
    [Game] public sealed class LootSpawnInterval : IComponent { public float Value; }
    [Game] public sealed class LootToSpawn : IComponent { public List<LootTypeId> Value; }
}