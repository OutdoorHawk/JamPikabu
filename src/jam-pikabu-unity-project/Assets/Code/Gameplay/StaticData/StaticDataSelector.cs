using UnityEngine;

namespace Code.Gameplay.StaticData
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(BuildConfigStaticData), fileName = "BuildConfig")]
    public class BuildConfigStaticData : BaseStaticData
    {
        [SerializeField] private BuildConfigType _buildConfigType;
        [SerializeField] private int _syncIntervalSeconds = 120;

        public BuildConfigType ConfigType => _buildConfigType;

        public int SyncIntervalSeconds => _syncIntervalSeconds;
    }
}