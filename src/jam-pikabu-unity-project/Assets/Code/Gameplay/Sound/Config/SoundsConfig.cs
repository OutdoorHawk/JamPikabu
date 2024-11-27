using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.Sound.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(SoundsConfig), fileName = "Sounds")]
    public class SoundsConfig : ScriptableObject
    {
        [SerializeField] private List<SoundConfig> _sounds;

        public List<SoundConfig> Sounds => _sounds;
    }
}