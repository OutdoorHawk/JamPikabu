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

        [Inject]
        private void Construct(IStaticDataService staticData)
        {
            _staticData = staticData;
        }

        protected override void Awake()
        {
            base.Awake();
            
            LootSetup lootSetup = _staticData.GetStaticData<LootStaticData>().GetConfig(Entity.LootTypeId);
            _sprite.sprite = lootSetup.Icon;
            var capsuleCollider2D = _sprite.gameObject.AddComponent<CapsuleCollider2D>();
            capsuleCollider2D.size = Vector3.one * lootSetup.Size;
            capsuleCollider2D.sharedMaterial = capsuleCollider2D.attachedRigidbody.sharedMaterial;
        }

        private void Start()
        {
           
        }
    }
}