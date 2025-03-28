//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class InputMatcher {

    static Entitas.IMatcher<InputEntity> _matcherEnter;

    public static Entitas.IMatcher<InputEntity> Enter {
        get {
            if (_matcherEnter == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.Enter);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherEnter = matcher;
            }

            return _matcherEnter;
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

    public InputEntity enterEntity { get { return GetGroup(InputMatcher.Enter).GetSingleEntity(); } }

    public bool isEnter {
        get { return enterEntity != null; }
        set {
            var entity = enterEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isEnter = true;
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

    static readonly EnterComponent enterComponent = new EnterComponent();

    public bool isEnter {
        get { return HasComponent(InputComponentsLookup.Enter); }
        set {
            if (value != isEnter) {
                var index = InputComponentsLookup.Enter;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : enterComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
