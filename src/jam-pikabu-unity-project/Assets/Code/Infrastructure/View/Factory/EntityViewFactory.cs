using Code.Infrastructure.AssetManagement.AssetProvider;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.View.Factory
{
    public class EntityViewFactory : IEntityViewFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IInstantiator _instantiator;
        private readonly Vector3 _farAway = new(-999, 999, 0);

        public EntityViewFactory(IAssetProvider assetProvider, IInstantiator instantiator)
        {
            _assetProvider = assetProvider;
            _instantiator = instantiator;
        }

        public EntityView CreateViewForEntityFromResources(GameEntity entity)
        {
            EntityView viewPrefab = _assetProvider.LoadAssetFromResources<EntityView>(entity.ViewPathResources);
            EntityView view = _instantiator.InstantiatePrefabForComponent<EntityView>(
                viewPrefab,
                position: GetStartPosition(entity),
                rotation: GetStartRotation(entity),
                parentTransform: null);

            view.SetEntity(entity);

            return view;
        }

        public EntityView CreateViewForEntityFromPrefab(GameEntity entity)
        {
            GameObject view = _instantiator.InstantiatePrefab(
                entity.ViewPrefab,
                position: GetStartPosition(entity),
                rotation: GetStartRotation(entity),
                parentTransform: null);
            
            EntityView viewComponent = view.GetComponent<EntityView>();
            viewComponent.SetEntity(entity);

            return viewComponent;
        }

        private Quaternion GetStartRotation(GameEntity entity)
        {
            if (entity.hasStartRotation)
                return Quaternion.Euler(entity.StartRotation);
            
            return Quaternion.identity;
        }

        private Vector3 GetStartPosition(GameEntity entity)
        {
            if (entity.hasStartWorldPosition)
                return entity.StartWorldPosition;

            return _farAway;
        }
    }
}