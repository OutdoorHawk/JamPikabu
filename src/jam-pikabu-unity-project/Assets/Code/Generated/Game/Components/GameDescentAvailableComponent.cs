//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherDescentAvailable;

    public static Entitas.IMatcher<GameEntity> DescentAvailable {
        get {
            if (_matcherDescentAvailable == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.DescentAvailable);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherDescentAvailable = matcher;
            }

            return _matcherDescentAvailable;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly Code.Gameplay.Features.GrapplingHook.DescentAvailable descentAvailableComponent = new Code.Gameplay.Features.GrapplingHook.DescentAvailable();

    public bool isDescentAvailable {
        get { return HasComponent(GameComponentsLookup.DescentAvailable); }
        set {
            if (value != isDescentAvailable) {
                var index = GameComponentsLookup.DescentAvailable;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : descentAvailableComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
