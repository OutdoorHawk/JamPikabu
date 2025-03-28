//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherRoundDuration;

    public static Entitas.IMatcher<GameEntity> RoundDuration {
        get {
            if (_matcherRoundDuration == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.RoundDuration);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherRoundDuration = matcher;
            }

            return _matcherRoundDuration;
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

    public Code.Gameplay.Features.RoundState.RoundDuration roundDuration { get { return (Code.Gameplay.Features.RoundState.RoundDuration)GetComponent(GameComponentsLookup.RoundDuration); } }
    public float RoundDuration { get { return roundDuration.Value; } }
    public bool hasRoundDuration { get { return HasComponent(GameComponentsLookup.RoundDuration); } }

    public GameEntity AddRoundDuration(float newValue) {
        var index = GameComponentsLookup.RoundDuration;
        var component = (Code.Gameplay.Features.RoundState.RoundDuration)CreateComponent(index, typeof(Code.Gameplay.Features.RoundState.RoundDuration));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceRoundDuration(float newValue) {
        var index = GameComponentsLookup.RoundDuration;
        var component = (Code.Gameplay.Features.RoundState.RoundDuration)CreateComponent(index, typeof(Code.Gameplay.Features.RoundState.RoundDuration));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveRoundDuration() {
        RemoveComponent(GameComponentsLookup.RoundDuration);
        return this;
    }
}
