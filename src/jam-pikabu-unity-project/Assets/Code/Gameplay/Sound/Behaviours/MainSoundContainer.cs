using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Code.Gameplay.Sound.Behaviours
{
    public class MainSoundContainer : MonoBehaviour
    {
        public AudioSource MusicSource;
        public AudioSource RareSFXSource;
        public List<AudioSource> SfxList = new();

        public AudioMixer Mixer;
        public AudioMixerGroup Music;
        public AudioMixerGroup CommonSfx;
        public AudioMixerGroup RareSfx;
    }
}