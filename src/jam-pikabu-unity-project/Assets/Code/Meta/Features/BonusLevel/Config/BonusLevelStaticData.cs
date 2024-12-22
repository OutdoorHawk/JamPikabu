using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.Features.BonusLevel.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(BonusLevelStaticData), fileName = "BonusLevel")]
    public class BonusLevelStaticData : BaseStaticData<BonusLevelData>
    {
        
    }
}