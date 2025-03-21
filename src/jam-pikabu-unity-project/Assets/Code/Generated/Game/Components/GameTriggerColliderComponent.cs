//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherTriggerCollider;

    public static Entitas.IMatcher<GameEntity> TriggerCollider {
        get {
            if (_matcherTriggerCollider == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TriggerCollider);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTriggerCollider = matcher;
            }

            return _matcherTriggerCollider;
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

    public Code.Gameplay.Features.CollidingView.TriggerCollider triggerCollider { get { return (Code.Gameplay.Features.CollidingView.TriggerCollider)GetComponent(GameComponentsLookup.TriggerCollider); } }
    public UnityEngine.Collider TriggerCollider { get { return triggerCollider.Value; } }
    public bool hasTriggerCollider { get { return HasComponent(GameComponentsLookup.TriggerCollider); } }

    public GameEntity AddTriggerCollider(UnityEngine.Collider newValue) {
        var index = GameComponentsLookup.TriggerCollider;
        var component = (Code.Gameplay.Features.CollidingView.TriggerCollider)CreateComponent(index, typeof(Code.Gameplay.Features.CollidingView.TriggerCollider));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceTriggerCollider(UnityEngine.Collider newValue) {
        var index = GameComponentsLookup.TriggerCollider;
        var component = (Code.Gameplay.Features.CollidingView.TriggerCollider)CreateComponent(index, typeof(Code.Gameplay.Features.CollidingView.TriggerCollider));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveTriggerCollider() {
        RemoveComponent(GameComponentsLookup.TriggerCollider);
        return this;
    }
}
