using UnityEngine;

namespace Code.Infrastructure.View
{
  public abstract class EntityDependant : MonoBehaviour
  {
    public EntityView EntityView;

    public GameEntity Entity => EntityView != null ? EntityView.Entity : null;

    private void Awake()
    {
        EntityView ??= GetComponent<EntityView>();
    }
  }
}