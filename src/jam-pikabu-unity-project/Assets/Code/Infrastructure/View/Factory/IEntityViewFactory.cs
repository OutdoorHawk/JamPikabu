using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.View.Factory
{
    public interface IEntityViewFactory
    {
        EntityView CreateViewForEntityFromResources(GameEntity entity);
        EntityView CreateViewForEntityFromPrefab(GameEntity entity);
        UniTask<EntityView> CreateViewForEntityFromAddressables(GameEntity entity);
    }
}