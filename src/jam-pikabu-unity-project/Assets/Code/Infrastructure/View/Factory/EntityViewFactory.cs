using Code.Infrastructure.AssetManagement.AssetProvider;
using Cysharp.Threading.Tasks;
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
            EntityView viewPrefab = _assetProvider.LoadAssetFromResouses<EntityView>(entity.ViewPathResources);
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

        public async UniTask<EntityView> CreateViewForEntityFromAddressables(GameEntity entity)
        {
            GameObject viewPrefab = await _assetProvider.Load<GameObject>(entity.ViewPathAddressables);

            GameObject view = _instantiator.InstantiatePrefab(
                viewPrefab,
                position: GetStartPosition(entity),
                rotation: GetStartRotation(entity),
                parentTransform: entity.hasTargetParent ? entity.TargetParent : null);

            EntityView viewComponent = view.GetComponent<EntityView>();
            viewComponent.SetEntity(entity);

            entity.isViewPrefabLoadProcessing = false;
            return viewComponent;
        }

        private Quaternion GetStartRotation(GameEntity entity)
        {
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