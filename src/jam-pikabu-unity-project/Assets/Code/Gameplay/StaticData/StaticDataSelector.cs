using UnityEngine;

namespace Code.Gameplay.StaticData
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(BuildConfigStaticData), fileName = "BuildConfig")]
    public class BuildConfigStaticData : BaseStaticData
    {
        [SerializeField] private BuildConfigType _buildConfigType;

        public BuildConfigType ConfigType => _buildConfigType;
    }
}