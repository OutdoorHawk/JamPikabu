using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Code.Gameplay.Sound.CustomData
{
    public static class MixerGroupFader
    {
        private const float MIN_VOLUME = 0.0001f;
        private const float MAX_VOLUME = 1f;
        
        public static IEnumerator StartFadeVolumeParameter(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
        {
            float currentTime = 0f;
            audioMixer.GetFloat(exposedParam, out float currentVolume);
            currentVolume = Mathf.Pow(10, currentVolume / 20);
            targetVolume = Mathf.Clamp(targetVolume, MIN_VOLUME, MAX_VOLUME);

            while (currentTime <= duration)
            {
                currentTime += Time.unscaledDeltaTime;
                float newVolume = Mathf.Lerp(currentVolume, targetVolume, currentTime / duration);
                float volumeInDb = Mathf.Log10(newVolume) * 20;
                audioMixer.SetFloat(exposedParam, volumeInDb);
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
        }
    }
}
