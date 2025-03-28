//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherRoundTimeLeft;

    public static Entitas.IMatcher<GameEntity> RoundTimeLeft {
        get {
            if (_matcherRoundTimeLeft == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.RoundTimeLeft);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherRoundTimeLeft = matcher;
            }

            return _matcherRoundTimeLeft;
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

    public Code.Gameplay.Features.RoundState.RoundTimeLeft roundTimeLeft { get { return (Code.Gameplay.Features.RoundState.RoundTimeLeft)GetComponent(GameComponentsLookup.RoundTimeLeft); } }
    public float RoundTimeLeft { get { return roundTimeLeft.Value; } }
    public bool hasRoundTimeLeft { get { return HasComponent(GameComponentsLookup.RoundTimeLeft); } }

    public GameEntity AddRoundTimeLeft(float newValue) {
        var index = GameComponentsLookup.RoundTimeLeft;
        var component = (Code.Gameplay.Features.RoundState.RoundTimeLeft)CreateComponent(index, typeof(Code.Gameplay.Features.RoundState.RoundTimeLeft));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceRoundTimeLeft(float newValue) {
        var index = GameComponentsLookup.RoundTimeLeft;
        var component = (Code.Gameplay.Features.RoundState.RoundTimeLeft)CreateComponent(index, typeof(Code.Gameplay.Features.RoundState.RoundTimeLeft));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveRoundTimeLeft() {
        RemoveComponent(GameComponentsLookup.RoundTimeLeft);
        return this;
    }
}
