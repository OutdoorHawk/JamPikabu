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

        private IStaticDataService _staticData;
        private CapsuleCollider2D _capsuleCollider2D;
        private PolygonCollider2D _polygonCollider2D;
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
            transform.localScale = Vector3.one * lootSetup.Size;
            _polygonCollider2D = _sprite.gameObject.AddComponent<PolygonCollider2D>();
            _polygonCollider2D.useDelaunayMesh = true;
            //_capsuleCollider2D.size = Vector3.one * lootSetup.ColliderSize;
            _polygonCollider2D.sharedMaterial = _polygonCollider2D.attachedRigidbody.sharedMaterial;
            _collisionRegistry.Register(_polygonCollider2D.GetInstanceID(), Entity);
        }
    }
}