using Entitas;

namespace Code.Gameplay.Features.Currency
{
  [Game] public sealed class CurrencyStorage : IComponent { }
  [Game] public sealed class Gold : IComponent { public int Value; }
  [Game] public sealed class AddGoldRequest : IComponent { }
  [Game] public sealed class Withdraw : IComponent { public int Value; }
  [Game] public sealed class CurrencyTypeIdComponent : IComponent { public CurrencyTypeId Value; }
}