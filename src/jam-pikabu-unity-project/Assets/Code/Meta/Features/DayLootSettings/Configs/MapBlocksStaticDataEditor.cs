using Sirenix.OdinInspector;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    public partial class MapBlocksStaticData
    {
        [FoldoutGroup("Editor")]
        [Button]
        private void SetupStarsNeedToUnlock()
        {
            for (int i = 0; i < Configs.Count; i++)
            {
                Configs[i].StarsNeedToUnlock = DAYS_IN_BLOCK * i * 3;
            }
        }
    }
}