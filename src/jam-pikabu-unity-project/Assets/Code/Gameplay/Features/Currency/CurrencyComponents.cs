using Entitas;

namespace Code.Gameplay.Features.Currency
{
  [Game] public sealed class CurrencyStorage : IComponent { }
  [Game] public sealed class Gold : IComponent { public int Value; }
  [Game] public sealed class EarnedInDay : IComponent { public int Value; }
  [Game] public sealed class Plus : IComponent { public int Value; }
  [Game] public sealed class Minus : IComponent { public int Value; }
  [Game] public sealed class AddCurrencyRequest : IComponent { }
  [Game] public sealed class SyncMetaStorageRequest : IComponent { }
  [Game] public sealed class CurrencyAmount : IComponent { public int Value; }
  [Game] public sealed class Withdraw : IComponent { public int Value; }
  [Game] public sealed class CurrencyTypeIdComponent : IComponent { public CurrencyTypeId Value; }
}