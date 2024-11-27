using Code.Infrastructure.View;
using Entitas;
using UnityEngine;

namespace Code.Common
{
    [Game, Meta] public class Destructed : IComponent { }
    [Game] public class View : IComponent { public IEntityView Value; }
    [Game] public class ViewPathResources : IComponent { public string Value; }
    [Game] public class ViewPrefab : IComponent { public EntityView Value; }
    [Game] public class ViewPathAddressables : IComponent { public string Value; }
    [Game] public class TargetParent : IComponent { public Transform Value; }
    [Game] public class ViewPrefabLoadProcessing : IComponent { }
    [Game] public class SelfDestructTimer : IComponent { public float Value; }
    [Game] public class Radius : IComponent { public float Value; }
    
    
}