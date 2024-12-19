using Code.Gameplay.Common.Collisions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Loot.Behaviours
{
    public class LootItem : EntityDependant
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private SpriteRenderer _colliderRenderer;

        private IStaticDataService _staticData;
        private PolygonCollider2D _collider2D;
        private Collider2D _collider;
        private ICollisionRegistry _collisionRegistry;

        public SpriteRenderer Sprite => _sprite;

        [Inject]
        private void Construct(IStaticDataService staticData, ICollisionRegistry collisionRegistry)
        {
            _collisionRegistry = collisionRegistry;
            _staticData = staticData;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            LootSetup lootSetup = _staticData.GetStaticData<LootSettingsStaticData>().GetConfig(Entity.LootTypeId);
            _sprite.sprite = lootSetup.Icon;
            _colliderRenderer.sprite = lootSetup.Icon;
            transform.localScale = Vector3.one * lootSetup.Size;
            CreateCollider(lootSetup);
            Destroy(_colliderRenderer);
        }

        private void CreateCollider(LootSetup lootSetup)
        {
            AddCapsule(lootSetup);
            _collider.sharedMaterial = _collider.attachedRigidbody.sharedMaterial;
            
            _collisionRegistry.Register(_collider.GetInstanceID(), Entity);
        }

        private void AddCapsule(LootSetup lootSetup)
        {
            var capsuleCollider2D = _colliderRenderer.gameObject.AddComponent<CapsuleCollider2D>();
            capsuleCollider2D.size = Vector3.one * lootSetup.ColliderSize;
            _collider = capsuleCollider2D;
        }
    }
}