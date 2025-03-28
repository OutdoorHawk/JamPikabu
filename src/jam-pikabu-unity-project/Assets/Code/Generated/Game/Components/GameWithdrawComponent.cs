//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherWithdraw;

    public static Entitas.IMatcher<GameEntity> Withdraw {
        get {
            if (_matcherWithdraw == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Withdraw);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherWithdraw = matcher;
            }

            return _matcherWithdraw;
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

    public Code.Gameplay.Features.Currency.Withdraw withdraw { get { return (Code.Gameplay.Features.Currency.Withdraw)GetComponent(GameComponentsLookup.Withdraw); } }
    public int Withdraw { get { return withdraw.Value; } }
    public bool hasWithdraw { get { return HasComponent(GameComponentsLookup.Withdraw); } }

    public GameEntity AddWithdraw(int newValue) {
        var index = GameComponentsLookup.Withdraw;
        var component = (Code.Gameplay.Features.Currency.Withdraw)CreateComponent(index, typeof(Code.Gameplay.Features.Currency.Withdraw));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceWithdraw(int newValue) {
        var index = GameComponentsLookup.Withdraw;
        var component = (Code.Gameplay.Features.Currency.Withdraw)CreateComponent(index, typeof(Code.Gameplay.Features.Currency.Withdraw));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveWithdraw() {
        RemoveComponent(GameComponentsLookup.Withdraw);
        return this;
    }
}
