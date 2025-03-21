//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherGrapplingHookBehaviour;

    public static Entitas.IMatcher<GameEntity> GrapplingHookBehaviour {
        get {
            if (_matcherGrapplingHookBehaviour == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GrapplingHookBehaviour);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherGrapplingHookBehaviour = matcher;
            }

            return _matcherGrapplingHookBehaviour;
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

    public Code.Gameplay.Features.GrapplingHook.GrapplingHookBehaviourComponent grapplingHookBehaviour { get { return (Code.Gameplay.Features.GrapplingHook.GrapplingHookBehaviourComponent)GetComponent(GameComponentsLookup.GrapplingHookBehaviour); } }
    public Code.Gameplay.Features.GrapplingHook.Behaviours.GrapplingHookBehaviour GrapplingHookBehaviour { get { return grapplingHookBehaviour.Value; } }
    public bool hasGrapplingHookBehaviour { get { return HasComponent(GameComponentsLookup.GrapplingHookBehaviour); } }

    public GameEntity AddGrapplingHookBehaviour(Code.Gameplay.Features.GrapplingHook.Behaviours.GrapplingHookBehaviour newValue) {
        var index = GameComponentsLookup.GrapplingHookBehaviour;
        var component = (Code.Gameplay.Features.GrapplingHook.GrapplingHookBehaviourComponent)CreateComponent(index, typeof(Code.Gameplay.Features.GrapplingHook.GrapplingHookBehaviourComponent));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceGrapplingHookBehaviour(Code.Gameplay.Features.GrapplingHook.Behaviours.GrapplingHookBehaviour newValue) {
        var index = GameComponentsLookup.GrapplingHookBehaviour;
        var component = (Code.Gameplay.Features.GrapplingHook.GrapplingHookBehaviourComponent)CreateComponent(index, typeof(Code.Gameplay.Features.GrapplingHook.GrapplingHookBehaviourComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveGrapplingHookBehaviour() {
        RemoveComponent(GameComponentsLookup.GrapplingHookBehaviour);
        return this;
    }
}
