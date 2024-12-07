using Code.Gameplay.StaticData;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(LootStaticData), fileName = "Loot")]
    public class LootStaticData : BaseStaticData<LootSetup>
    {
        public float LootSpawnInterval = 0.05f;
        public float LootSpawnConveyorInterval = 0.2f;
        public float LootSpawnStartDelay = 0.5f;
        public float MaxLootAmount = 50;
        public float CollectFlyAnimationDuration = 0.5f;
        public Vector2 CollectFlyMinMaxJump = new(-1, 2);
        public EntityView LootItemUI;
        public EntityView LootItem;
        
        public override void OnConfigInit()
        {
            base.OnConfigInit();

            AddIndex(setup => (int)setup.Type);
        }

        public LootSetup GetConfig(LootTypeId typeId)
        {
            return GetByKey((int)typeId);
        }
    }
}