using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.Sound.Config
{
    [Serializable]
    public class SoundConfig
    {
        public SoundTypeId SoundType;
        public SoundSetup Data;
    }
    
    [Serializable]
    public class SoundSetup
    {
        public AudioClip[] Clips;
        [Tooltip("Mixer channel")] public SoundSourceTypeId SourceType = SoundSourceTypeId.CommonSfx;
        [Tooltip("Clip volume in %"), Range(0, 1)] public float Volume = 1f;
        public bool RandomizePitch = false;
        [ShowIf(nameof(RandomizePitch))] public float PitchMin = 0.8f;
        [ShowIf(nameof(RandomizePitch))] public float PitchMax = 1.2f;
    }
}