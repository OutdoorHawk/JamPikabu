//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class MetaMatcher {

    static Entitas.IMatcher<MetaEntity> _matcherFreeUpgradeRequest;

    public static Entitas.IMatcher<MetaEntity> FreeUpgradeRequest {
        get {
            if (_matcherFreeUpgradeRequest == null) {
                var matcher = (Entitas.Matcher<MetaEntity>)Entitas.Matcher<MetaEntity>.AllOf(MetaComponentsLookup.FreeUpgradeRequest);
                matcher.componentNames = MetaComponentsLookup.componentNames;
                _matcherFreeUpgradeRequest = matcher;
            }

            return _matcherFreeUpgradeRequest;
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
public partial class MetaEntity {

    static readonly Code.Meta.Features.MapBlocks.FreeUpgradeRequest freeUpgradeRequestComponent = new Code.Meta.Features.MapBlocks.FreeUpgradeRequest();

    public bool isFreeUpgradeRequest {
        get { return HasComponent(MetaComponentsLookup.FreeUpgradeRequest); }
        set {
            if (value != isFreeUpgradeRequest) {
                var index = MetaComponentsLookup.FreeUpgradeRequest;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : freeUpgradeRequestComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
