//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherCollisionId;

    public static Entitas.IMatcher<GameEntity> CollisionId {
        get {
            if (_matcherCollisionId == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.CollisionId);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherCollisionId = matcher;
            }

            return _matcherCollisionId;
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

    public Code.Gameplay.Common.CollisionId collisionId { get { return (Code.Gameplay.Common.CollisionId)GetComponent(GameComponentsLookup.CollisionId); } }
    public int CollisionId { get { return collisionId.Value; } }
    public bool hasCollisionId { get { return HasComponent(GameComponentsLookup.CollisionId); } }

    public GameEntity AddCollisionId(int newValue) {
        var index = GameComponentsLookup.CollisionId;
        var component = (Code.Gameplay.Common.CollisionId)CreateComponent(index, typeof(Code.Gameplay.Common.CollisionId));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceCollisionId(int newValue) {
        var index = GameComponentsLookup.CollisionId;
        var component = (Code.Gameplay.Common.CollisionId)CreateComponent(index, typeof(Code.Gameplay.Common.CollisionId));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveCollisionId() {
        RemoveComponent(GameComponentsLookup.CollisionId);
        return this;
    }
}
