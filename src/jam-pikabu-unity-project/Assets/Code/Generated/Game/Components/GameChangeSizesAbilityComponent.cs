//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherChangeSizesAbility;

    public static Entitas.IMatcher<GameEntity> ChangeSizesAbility {
        get {
            if (_matcherChangeSizesAbility == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.ChangeSizesAbility);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherChangeSizesAbility = matcher;
            }

            return _matcherChangeSizesAbility;
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

    static readonly Code.Gameplay.Features.Abilities.ChangeSizesAbility changeSizesAbilityComponent = new Code.Gameplay.Features.Abilities.ChangeSizesAbility();

    public bool isChangeSizesAbility {
        get { return HasComponent(GameComponentsLookup.ChangeSizesAbility); }
        set {
            if (value != isChangeSizesAbility) {
                var index = GameComponentsLookup.ChangeSizesAbility;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : changeSizesAbilityComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
