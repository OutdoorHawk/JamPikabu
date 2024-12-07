using System.Collections.Generic;
using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Features.RoundState.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(RoundStateStaticData), fileName = "RoundState")]
    public class RoundStateStaticData : BaseStaticData
    {
        public int StartGoldAmount = 50;
        
        public List<DayData> Days;
    }
}