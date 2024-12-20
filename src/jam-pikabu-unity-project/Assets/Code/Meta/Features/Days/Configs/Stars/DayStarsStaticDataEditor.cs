using Sirenix.OdinInspector;

namespace Code.Meta.Features.Days.Configs.Stars
{
    public partial class DayStarsStaticData
    {
        [FoldoutGroup("Editor")] public DaysStaticData DaysStaticData;
        
        [Button]
        private void CreateConfigsForEveryDay()
        {
            foreach (DayData dayData in DaysStaticData.Configs)
            {
                Configs.Add(new DayStarsSetup());
            }
        }
    }
}