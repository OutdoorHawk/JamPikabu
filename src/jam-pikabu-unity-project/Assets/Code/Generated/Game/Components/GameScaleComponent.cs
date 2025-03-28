//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherScale;

    public static Entitas.IMatcher<GameEntity> Scale {
        get {
            if (_matcherScale == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Scale);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherScale = matcher;
            }

            return _matcherScale;
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

    public Code.Gameplay.Features.CharacterStats.Scale scale { get { return (Code.Gameplay.Features.CharacterStats.Scale)GetComponent(GameComponentsLookup.Scale); } }
    public float Scale { get { return scale.Value; } }
    public bool hasScale { get { return HasComponent(GameComponentsLookup.Scale); } }

    public GameEntity AddScale(float newValue) {
        var index = GameComponentsLookup.Scale;
        var component = (Code.Gameplay.Features.CharacterStats.Scale)CreateComponent(index, typeof(Code.Gameplay.Features.CharacterStats.Scale));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceScale(float newValue) {
        var index = GameComponentsLookup.Scale;
        var component = (Code.Gameplay.Features.CharacterStats.Scale)CreateComponent(index, typeof(Code.Gameplay.Features.CharacterStats.Scale));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveScale() {
        RemoveComponent(GameComponentsLookup.Scale);
        return this;
    }
}
