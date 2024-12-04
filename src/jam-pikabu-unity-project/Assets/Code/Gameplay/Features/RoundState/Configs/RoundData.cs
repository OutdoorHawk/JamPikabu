using System;
using Code.Infrastructure.SceneLoading;

namespace Code.Gameplay.Features.RoundState.Configs
{
    [Serializable]
    public class RoundData
    {
        public int RoundId;
        public int PlayCost;
        public SceneTypeId SceneId;
    }
}