//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherNextOrderRequest;

    public static Entitas.IMatcher<GameEntity> NextOrderRequest {
        get {
            if (_matcherNextOrderRequest == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.NextOrderRequest);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherNextOrderRequest = matcher;
            }

            return _matcherNextOrderRequest;
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

    static readonly Code.Gameplay.Features.Orders.NextOrderRequest nextOrderRequestComponent = new Code.Gameplay.Features.Orders.NextOrderRequest();

    public bool isNextOrderRequest {
        get { return HasComponent(GameComponentsLookup.NextOrderRequest); }
        set {
            if (value != isNextOrderRequest) {
                var index = GameComponentsLookup.NextOrderRequest;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : nextOrderRequestComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
