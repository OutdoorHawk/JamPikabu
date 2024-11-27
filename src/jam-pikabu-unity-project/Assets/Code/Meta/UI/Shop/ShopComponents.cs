using Code.Progress;
using Entitas;

namespace Code.Meta.UI.Shop
{
  [Meta] public class ShopItemId : ISavedComponent { public int Value; }
  [Meta] public class Purchased : ISavedComponent { }
  [Meta] public class BuyRequestComponent : IComponent { }
}