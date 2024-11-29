// ReSharper disable CheckNamespace

using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Input] [Unique] public sealed class Input : IComponent { }
[Input] [Unique] public sealed class MouseAxisComponent : IComponent { public Vector2 Value; }
[Input] [Unique] public sealed class MovementAxisComponent : IComponent { public Vector2 Value; } 
[Input] [Unique] public sealed class FireComponent : IComponent { } 
[Input] [Unique] public sealed class JumpComponent : IComponent { } 
[Input] [Unique] public sealed class PauseInputComponent : IComponent { }
[Input] [Unique] public sealed class EscapeComponent : IComponent { } 
[Input] [Unique] public sealed class SprintComponent : IComponent { } 
[Input] [Unique] public sealed class SelectionComponent : IComponent { }
[Input] [Unique] public sealed class Selection1Component : IComponent { }
[Input] [Unique] public sealed class Selection2Component : IComponent { }
[Input] [Unique] public sealed class Selection3Component : IComponent { }
[Input] [Unique] public sealed class Selection4Component : IComponent { }
[Input] [Unique] public sealed class Selection5Component : IComponent { }
[Input] [Unique] public sealed class Selection6Component : IComponent { }
