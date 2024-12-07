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
            _sprite.gameObject.AddComponent<CapsuleCollider2D>();
        }

        private void Start()
        {
            LootSetup lootSetup = _staticData.GetStaticData<LootStaticData>().GetConfig(Entity.LootTypeId);
            _sprite.sprite = lootSetup.Icon;
        }
    }
}