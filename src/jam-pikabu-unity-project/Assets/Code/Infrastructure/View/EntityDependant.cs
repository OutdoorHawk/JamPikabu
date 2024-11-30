using UnityEngine;

namespace Code.Infrastructure.View
{
  public abstract class EntityDependant : MonoBehaviour
  {
    public EntityView EntityView;

    public GameEntity Entity => EntityView != null ? EntityView.Entity : null;

    protected virtual void Awake()
    {
        EntityView ??= GetComponent<EntityView>();
    }
  }
}