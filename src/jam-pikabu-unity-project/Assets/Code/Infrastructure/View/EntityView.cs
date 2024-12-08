using System;
using Code.Gameplay.Common.Collisions;
using Code.Infrastructure.View.Registrars;
using Entitas.VisualDebugging.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.View
{
    [SelectionBase]
    public class EntityView : MonoBehaviour, IEntityView
    {
        private GameEntity _entity;
        private ICollisionRegistry _collisionRegistry;

        public GameEntity Entity => _entity;

        [Inject]
        private void Construct(ICollisionRegistry collisionRegistry)
        {
            _collisionRegistry = collisionRegistry;
        }

        public void SetEntity(GameEntity gameEntity)
        {
            _entity = gameEntity;
            _entity.AddView(this);
            _entity.Retain(this);

            InitRegistrars();
            RegisterCollisions();
        }

        public void ReleaseEntity()
        {
            UnregisterCollisions();
            CleanUpRegistrars();

            _entity.Release(this);
            _entity = null;
        }

        private void InitRegistrars()
        {
            foreach (IEntityComponentRegistrar registrar in GetComponentsInChildren<IEntityComponentRegistrar>())
                registrar.RegisterComponents();
        }

        private void CleanUpRegistrars()
        {
            foreach (IEntityComponentRegistrar registrar in GetComponentsInChildren<IEntityComponentRegistrar>())
                registrar.UnregisterComponents();
        }

        private void RegisterCollisions()
        {
            foreach (var coll in GetComponentsInChildren<Collider2D>(includeInactive: true))
                _collisionRegistry.Register(coll.GetInstanceID(), _entity);
        }

        private void UnregisterCollisions()
        {
            if (this == null)
                return;
            
            foreach (var coll in GetComponentsInChildren<Collider2D>())
                _collisionRegistry.Unregister(coll.GetInstanceID());
        }

#if UNITY_EDITOR
        [Button]
        private void SelectEntity()
        {
            EntityBehaviour[] entitasBehavior = FindObjectsOfType<EntityBehaviour>();
            foreach (var entitasBehaviour in entitasBehavior)
            {
                if (entitasBehaviour.entity != Entity)
                    continue;

                UnityEditor.Selection.activeObject = entitasBehaviour;
                return;
            }

            Debug.LogWarning("Entity not found!");
        }
#endif
    }
}