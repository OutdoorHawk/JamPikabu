using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    public partial class MapBlocksStaticData
    {
        [FoldoutGroup("Editor")] public int BaseStars = 3;
        [FoldoutGroup("Editor")] public float DifficultyGrowth = 0.25f;

        [FoldoutGroup("Editor")]
        [Button]
        private void SetupStarsNeedToUnlock()
        {
            int cumulativeStars = 0; // Накопленные звезды от предыдущих блоков

            for (int i = 0; i < Configs.Count; i++)
            {
                MapBlockData mapBlockData = Configs[i];

                if (i == 0)
                {
                    // Для первого блока всегда 0 звезд
                    mapBlockData.StarsNeedToUnlock = 0;
                }
                else
                {
                    // Расчет требуемого количества звезд для текущего блока
                    int maxPreviousStars = cumulativeStars; // Максимум звезд, которые можно было заработать до текущего блока
                    int requiredStars = Mathf.Clamp((int)(BaseStars + cumulativeStars * DifficultyGrowth), 0, maxPreviousStars);

                    // Устанавливаем звезды для разблокировки текущего блока
                    mapBlockData.StarsNeedToUnlock = requiredStars;
                }

                // Обновляем накопленное количество звезд
                cumulativeStars += DAYS_IN_BLOCK * 3; // Максимум звезд за текущий блок
            }
        }

        /*[FoldoutGroup("Editor")]
        [Button]
        private void SetupStarsNeedToUnlock()
        {
            for (int i = 0; i < Configs.Count; i++)
            {
                Configs[i].StarsNeedToUnlock = DAYS_IN_BLOCK * i * 3;
            }
        }*/
    }
}