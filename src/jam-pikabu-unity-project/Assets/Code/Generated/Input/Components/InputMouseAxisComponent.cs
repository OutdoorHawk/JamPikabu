//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class InputMatcher {

    static Entitas.IMatcher<InputEntity> _matcherMouseAxis;

    public static Entitas.IMatcher<InputEntity> MouseAxis {
        get {
            if (_matcherMouseAxis == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.MouseAxis);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherMouseAxis = matcher;
            }

            return _matcherMouseAxis;
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

    public InputEntity mouseAxisEntity { get { return GetGroup(InputMatcher.MouseAxis).GetSingleEntity(); } }
    public MouseAxisComponent mouseAxis { get { return mouseAxisEntity.mouseAxis; } }
    public UnityEngine.Vector2 MouseAxis { get { return mouseAxis.Value; } }
    public bool hasMouseAxis { get { return mouseAxisEntity != null; } }

    public InputEntity SetMouseAxis(UnityEngine.Vector2 newValue) {
        if (hasMouseAxis) {
            throw new Entitas.EntitasException("Could not set MouseAxis!\n" + this + " already has an entity with MouseAxisComponent!",
                "You should check if the context already has a mouseAxisEntity before setting it or use context.ReplaceMouseAxis().");
        }
        var entity = CreateEntity();
        entity.AddMouseAxis(newValue);
        return entity;
    }

    public void ReplaceMouseAxis(UnityEngine.Vector2 newValue) {
        var entity = mouseAxisEntity;
        if (entity == null) {
            entity = SetMouseAxis(newValue);
        } else {
            entity.ReplaceMouseAxis(newValue);
        }
    }

    public void RemoveMouseAxis() {
        mouseAxisEntity.Destroy();
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

    public MouseAxisComponent mouseAxis { get { return (MouseAxisComponent)GetComponent(InputComponentsLookup.MouseAxis); } }
    public UnityEngine.Vector2 MouseAxis { get { return mouseAxis.Value; } }
    public bool hasMouseAxis { get { return HasComponent(InputComponentsLookup.MouseAxis); } }

    public InputEntity AddMouseAxis(UnityEngine.Vector2 newValue) {
        var index = InputComponentsLookup.MouseAxis;
        var component = (MouseAxisComponent)CreateComponent(index, typeof(MouseAxisComponent));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public InputEntity ReplaceMouseAxis(UnityEngine.Vector2 newValue) {
        var index = InputComponentsLookup.MouseAxis;
        var component = (MouseAxisComponent)CreateComponent(index, typeof(MouseAxisComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public InputEntity RemoveMouseAxis() {
        RemoveComponent(InputComponentsLookup.MouseAxis);
        return this;
    }
}
