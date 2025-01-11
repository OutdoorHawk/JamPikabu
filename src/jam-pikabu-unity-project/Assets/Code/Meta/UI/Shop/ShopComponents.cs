using Entitas;

namespace Code.Meta.UI.Shop
{
  [Meta] public class ShopItemId : IComponent { public int Value; }
  [Meta] public class Purchased : IComponent { }
  [Meta] public class Consumable : IComponent { }
  [Meta] public class BuyRequestComponent : IComponent { }
}