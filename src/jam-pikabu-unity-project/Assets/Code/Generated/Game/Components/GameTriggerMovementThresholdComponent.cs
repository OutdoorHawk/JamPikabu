//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherTriggerMovementThreshold;

    public static Entitas.IMatcher<GameEntity> TriggerMovementThreshold {
        get {
            if (_matcherTriggerMovementThreshold == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TriggerMovementThreshold);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTriggerMovementThreshold = matcher;
            }

            return _matcherTriggerMovementThreshold;
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

    public Code.Gameplay.Features.GrapplingHook.TriggerMovementThreshold triggerMovementThreshold { get { return (Code.Gameplay.Features.GrapplingHook.TriggerMovementThreshold)GetComponent(GameComponentsLookup.TriggerMovementThreshold); } }
    public float TriggerMovementThreshold { get { return triggerMovementThreshold.Value; } }
    public bool hasTriggerMovementThreshold { get { return HasComponent(GameComponentsLookup.TriggerMovementThreshold); } }

    public GameEntity AddTriggerMovementThreshold(float newValue) {
        var index = GameComponentsLookup.TriggerMovementThreshold;
        var component = (Code.Gameplay.Features.GrapplingHook.TriggerMovementThreshold)CreateComponent(index, typeof(Code.Gameplay.Features.GrapplingHook.TriggerMovementThreshold));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceTriggerMovementThreshold(float newValue) {
        var index = GameComponentsLookup.TriggerMovementThreshold;
        var component = (Code.Gameplay.Features.GrapplingHook.TriggerMovementThreshold)CreateComponent(index, typeof(Code.Gameplay.Features.GrapplingHook.TriggerMovementThreshold));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveTriggerMovementThreshold() {
        RemoveComponent(GameComponentsLookup.TriggerMovementThreshold);
        return this;
    }
}
