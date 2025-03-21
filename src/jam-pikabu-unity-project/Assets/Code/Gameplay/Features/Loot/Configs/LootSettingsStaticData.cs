using Code.Gameplay.Features.Abilities;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.ABTesting;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(LootSettingsStaticData), fileName = "LootSettings")]
    public partial class LootSettingsStaticData : BaseStaticData<LootSettingsData>
    {
        public float LootSpawnInterval = 0.02f;
        public float LootSpawnConveyorInterval = 0.5f;
        public float LootSpawnStartDelay = 0.15f;
        public float DelayAfterLootSpawn = 0.4f;
        public int MaxIngredientLootAmount = 35;
        public int MaxEachExtraLootAmount = 2;
        public float CollectFlyAnimationDuration = 0.5f;

        public EntityView LootItemUI;
        public EntityView LootItem;

        public override void OnConfigInit()
        {
            base.OnConfigInit();

            AddIndex(setup => (int)setup.Type);
            InitExperiment();
        }

        public LootSettingsData GetConfig(LootTypeId typeId)
        {
            return GetByKey((int)typeId);
        }

        private void InitExperiment()
        {
            if (_abTestService.GetExperimentValue(ExperimentTagTypeId.TIMER_REPLACE)
                is not ExperimentValueTypeId.replace_timer_with_attempts)
                return;

            GetConfig(LootTypeId.Pumpkin).AbilityType = AbilityTypeId.DecreaseHookSize;
            GetConfig(LootTypeId.Pea).AbilityType = AbilityTypeId.IncreaseHookSize;
        }
    }
}