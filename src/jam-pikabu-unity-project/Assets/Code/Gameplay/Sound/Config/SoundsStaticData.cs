using System.Collections.Generic;
using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Sound.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(SoundsStaticData), fileName = "Sounds")]
    public class SoundsStaticData : BaseStaticData
    {
        [SerializeField] private List<SoundConfig> _sounds;

        private readonly Dictionary<SoundTypeId, SoundConfig> _soundsConfigs = new();

        public List<SoundConfig> Sounds => _sounds;

        public override void OnConfigInit()
        {
            base.OnConfigInit();

            LoadSounds();
        }

        private void LoadSounds()
        {
            foreach (var sounds in _sounds)
                _soundsConfigs.Add(sounds.SoundType, sounds);
        }

        public SoundConfig GetSoundConfig(SoundTypeId soundTypeId)
        {
            return _soundsConfigs.GetValueOrDefault(soundTypeId);
        }
    }
}