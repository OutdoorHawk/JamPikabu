//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherRoundStartRequest;

    public static Entitas.IMatcher<GameEntity> RoundStartRequest {
        get {
            if (_matcherRoundStartRequest == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.RoundStartRequest);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherRoundStartRequest = matcher;
            }

            return _matcherRoundStartRequest;
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

    static readonly Code.Gameplay.Features.RoundState.RoundStartRequest roundStartRequestComponent = new Code.Gameplay.Features.RoundState.RoundStartRequest();

    public bool isRoundStartRequest {
        get { return HasComponent(GameComponentsLookup.RoundStartRequest); }
        set {
            if (value != isRoundStartRequest) {
                var index = GameComponentsLookup.RoundStartRequest;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : roundStartRequestComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
