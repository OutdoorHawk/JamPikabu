using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Infrastructure.Ads.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(AdsStaticData), fileName = "Ads")]
    public class AdsStaticData : BaseStaticData
    {
        public int LevelsPassedToStartProfitAds = 3;
        public int LevelsPassedToStartInterstitialAds = 6;
        public int DoubleProfitMinGold = 10;
        public int DoubleProfitIntervalSeconds = 200;
        
        public bool TutorialBlockAds = false;
    }
}