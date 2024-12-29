using Entitas;

namespace Code.Gameplay.Features.LootSpawning
{
    [Game] public sealed class LootSpawner : IComponent { }
    [Game] public sealed class SingleSpawn : IComponent { }
    [Game] public sealed class ContinuousSpawn : IComponent { }
    [Game] public sealed class ConveyorSpawner : IComponent { }
    [Game] public sealed class LootSpawnInterval : IComponent { public float Value; }
}