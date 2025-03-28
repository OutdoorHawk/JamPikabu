//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherMultiPickupAbility;

    public static Entitas.IMatcher<GameEntity> MultiPickupAbility {
        get {
            if (_matcherMultiPickupAbility == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.MultiPickupAbility);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherMultiPickupAbility = matcher;
            }

            return _matcherMultiPickupAbility;
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

    static readonly Code.Gameplay.Features.Abilities.MultiPickupAbility multiPickupAbilityComponent = new Code.Gameplay.Features.Abilities.MultiPickupAbility();

    public bool isMultiPickupAbility {
        get { return HasComponent(GameComponentsLookup.MultiPickupAbility); }
        set {
            if (value != isMultiPickupAbility) {
                var index = GameComponentsLookup.MultiPickupAbility;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : multiPickupAbilityComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
