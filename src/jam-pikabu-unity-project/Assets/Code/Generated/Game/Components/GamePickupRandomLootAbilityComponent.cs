//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherPickupRandomLootAbility;

    public static Entitas.IMatcher<GameEntity> PickupRandomLootAbility {
        get {
            if (_matcherPickupRandomLootAbility == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.PickupRandomLootAbility);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherPickupRandomLootAbility = matcher;
            }

            return _matcherPickupRandomLootAbility;
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

    static readonly Code.Gameplay.Features.Abilities.PickupRandomLootAbility pickupRandomLootAbilityComponent = new Code.Gameplay.Features.Abilities.PickupRandomLootAbility();

    public bool isPickupRandomLootAbility {
        get { return HasComponent(GameComponentsLookup.PickupRandomLootAbility); }
        set {
            if (value != isPickupRandomLootAbility) {
                var index = GameComponentsLookup.PickupRandomLootAbility;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : pickupRandomLootAbilityComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
