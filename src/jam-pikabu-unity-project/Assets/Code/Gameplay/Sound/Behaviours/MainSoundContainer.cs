using System;
using System.Collections.Generic;
using Code.Common.Extensions;
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

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus == false)
            {
                foreach (AudioSource audioSource in SfxList)
                {
                    audioSource.DisableElement();
                }
            }
            else
            {
                foreach (AudioSource audioSource in SfxList)
                {
                    audioSource.EnableElement();
                }
            }
        }
    }
}