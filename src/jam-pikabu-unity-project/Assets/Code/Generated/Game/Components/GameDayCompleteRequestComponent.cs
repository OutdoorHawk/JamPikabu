//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherDayCompleteRequest;

    public static Entitas.IMatcher<GameEntity> DayCompleteRequest {
        get {
            if (_matcherDayCompleteRequest == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.DayCompleteRequest);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherDayCompleteRequest = matcher;
            }

            return _matcherDayCompleteRequest;
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

    static readonly Code.Gameplay.Features.RoundState.DayCompleteRequest dayCompleteRequestComponent = new Code.Gameplay.Features.RoundState.DayCompleteRequest();

    public bool isDayCompleteRequest {
        get { return HasComponent(GameComponentsLookup.DayCompleteRequest); }
        set {
            if (value != isDayCompleteRequest) {
                var index = GameComponentsLookup.DayCompleteRequest;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : dayCompleteRequestComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
