using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.View;
using Code.Meta.Features.Days.Service;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Loot.Behaviours
{
    public class LootItem : EntityDependant
    {
        [SerializeField] private SpriteRenderer _sprite;

        private IStaticDataService _staticData;
        private PolygonCollider2D _collider2D;
        private Collider2D _collider;
        private IDaysService _daysService;

        public SpriteRenderer Sprite => _sprite;

        [Inject]
        private void Construct(IStaticDataService staticData, IDaysService daysService)
        {
            _daysService = daysService;
            _staticData = staticData;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            InitDayType();
        }

        private void InitDayType()
        {
            if (_daysService.GetDayData().SceneId is SceneTypeId.NoGravityGameplayScene)
            {
                Entity.Rigidbody2D.gravityScale = 0;
            }
        }
    }
}