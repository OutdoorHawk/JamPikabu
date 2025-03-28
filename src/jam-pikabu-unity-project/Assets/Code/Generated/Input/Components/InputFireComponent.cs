//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class InputMatcher {

    static Entitas.IMatcher<InputEntity> _matcherFire;

    public static Entitas.IMatcher<InputEntity> Fire {
        get {
            if (_matcherFire == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.Fire);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherFire = matcher;
            }

            return _matcherFire;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class InputContext {

    public InputEntity fireEntity { get { return GetGroup(InputMatcher.Fire).GetSingleEntity(); } }

    public bool isFire {
        get { return fireEntity != null; }
        set {
            var entity = fireEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isFire = true;
                } else {
                    entity.Destroy();
                }
            }
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
public partial class InputEntity {

    static readonly FireComponent fireComponent = new FireComponent();

    public bool isFire {
        get { return HasComponent(InputComponentsLookup.Fire); }
        set {
            if (value != isFire) {
                var index = InputComponentsLookup.Fire;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : fireComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
