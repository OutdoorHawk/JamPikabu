using System;
using Code.Common.Extensions;
using Code.Gameplay.Common.Collisions;
using Code.Gameplay.Features.Abilities.Config;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.View;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Abilities.Behaviours
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class StickToKinematic : EntityDependant
    {
        public LayerMask StickLayerMask;

        private IStaticDataService _staticData;
        private ISoundService _soundService;
        private ICollisionRegistry _collisionRegistry;

        private Rigidbody2D _rb;
        private Transform _originParent;

        private float _stickDuration;

        [Inject]
        private void Construct(IStaticDataService staticData, ISoundService soundService, ICollisionRegistry collisionRegistry)
        {
            _collisionRegistry = collisionRegistry;
            _soundService = soundService;
            _staticData = staticData;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _originParent = transform.parent;

            AbilityData abilityData = _staticData
                .Get<AbilityStaticData>().GetDataByType(AbilityTypeId.StickyToHook);

            _stickDuration = abilityData.Value;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == null)
                return;

            if (collision.gameObject.layer.Matches(StickLayerMask) == false)
                return;

            var otherRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (otherRb == null || otherRb.bodyType != RigidbodyType2D.Kinematic)
                return;

            ActivateSticky(collision);
        }

        public void ReduceDuration(float duration)
        {
            _stickDuration -= duration;

            if (_stickDuration <= 0)
            {
                DisableSticky();
            }
        }

        private void ActivateSticky(Collision2D collision)
        {
            if (Entity == null)
                return;

            if (Entity.hasTarget)
                return;

            if (_collisionRegistry.TryGet(collision.collider.GetInstanceID(), out IEntity target) == false)
                return;

            EnableSticky(collision, target);
        }

        private void EnableSticky(Collision2D collision, IEntity target)
        {
            transform.SetParent(collision.collider.transform);
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _soundService.PlaySound(SoundTypeId.StickyAbility);
            Entity?.AddTarget(((GameEntity)target).Id);
        }

        private void DisableSticky()
        {
            transform.SetParent(_originParent);
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            if (Entity.hasTarget)
                Entity.RemoveTarget();
        }

        private void OnDestroy()
        {
            if (Entity == null)
                return;

            if (Entity.hasTarget)
                Entity.RemoveTarget();

            if (Entity.hasStickToKinematic) 
                Entity.RemoveStickToKinematic();
        }
    }
}