using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Infrastructure.Ads.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(AdsStaticData), fileName = "Ads")]
    public class AdsStaticData : BaseStaticData
    {
        public int LevelsPassedToStartAds = 2;
    }
}