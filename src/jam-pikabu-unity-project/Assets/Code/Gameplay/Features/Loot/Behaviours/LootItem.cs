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

        [Inject]
        private void Construct(IStaticDataService staticData)
        {
            _staticData = staticData;
        }

        protected override void Awake()
        {
            base.Awake();
            _capsuleCollider2D = _sprite.gameObject.AddComponent<CapsuleCollider2D>();
        }

        private void Start()
        {
            LootSetup lootSetup = _staticData.GetStaticData<LootStaticData>().GetConfig(Entity.LootTypeId);
            _sprite.sprite = lootSetup.Icon;
            transform.localScale = Vector3.one * lootSetup.Size;
            _capsuleCollider2D.size = Vector3.one * lootSetup.Size;
            _capsuleCollider2D.sharedMaterial = _capsuleCollider2D.attachedRigidbody.sharedMaterial;
        }
    }
}