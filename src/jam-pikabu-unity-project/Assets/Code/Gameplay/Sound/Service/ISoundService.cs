using System;
using Code.Gameplay.Settings;
using Code.Infrastructure.States.GameStateHandler;
using UnityEngine;

namespace Code.Gameplay.Sound.Service
{
    public interface ISoundService
    {
        private const float DefaultSoundFadeDuration = 0.1f;
        event Action OnSongUpdated;
        OrderType OrderType { get; }
        void SetVolume(SoundVolumeTypeId channelType, float value);
        void MuteVolume();
        void NextSong();
        void PreviousSong();
        AudioClip GetCurrentMusicClip();
        float GetVolumeForChannel(SoundVolumeTypeId channelType);
        void OnEnterLoadProgress();
        void OnExitLoadProgress();
        void OnEnterMainMenu();
        void PlaySound(SoundTypeId typeId);
        void PlaySound(SoundTypeId typeId, AudioSource audioSource);
        void PlayOneShotSound(SoundTypeId soundTypeId, AudioSource audioSource);
        void PlayOneShotSound(SoundTypeId soundTypeId);
        void StopSound(SoundTypeId typeId);
        void PlayMusic(SoundTypeId typeId);
        void StopMusic();
        void ResetVolume();
    }
}