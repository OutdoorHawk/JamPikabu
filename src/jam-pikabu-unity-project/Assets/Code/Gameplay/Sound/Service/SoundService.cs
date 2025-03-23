using System;
using System.Linq;
using System.Threading;
using Code.Common.Logger.Service;
using Code.Gameplay.Settings;
using Code.Gameplay.Sound.Behaviours;
using Code.Gameplay.Sound.Config;
using Code.Gameplay.StaticData;
using Code.Infrastructure.AssetManagement.AssetProvider;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Progress.Provider;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;
using static Code.Gameplay.Sound.SoundExtensions;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Sound.Service
{
    public class SoundService : MonoBehaviour,
        ISoundService,
        IMainMenuStateHandler,
        ILoadProgressStateHandler
    {
        private IStaticDataService _staticDataService;
        private IAssetProvider _assetProvider;
        private ILoggerService _loggerService;
        private SoundsStaticData _staticData;

        private MainSoundContainer _mainSoundContainer;
        private CancellationTokenSource _gamePlayMusicToken = new();
        private Coroutine _musicFadeCoroutine;
        private Coroutine _sfxFadeCoroutine;

        private Tweener _fadeTween;
        private bool _initialized;

        private const int MIN_MIXER_VOLUME = -60;
        private const int MAX_MIXER_VOLUME = 5;
        private const string MAIN_SOUND_CONTAINER = "MainSoundContainer";

        public event Action OnSongUpdated;

        [Inject]
        private void Construct
        (
            IStaticDataService staticDataService,
            IProgressProvider progressProvider,
            ILoggerService loggerService,
            IAssetProvider assetProvider
        )
        {
            _assetProvider = assetProvider;
            _loggerService = loggerService;
            _staticDataService = staticDataService;
        }

        #region Settings Window

        public void SetVolume(SoundVolumeTypeId channelType, float value)
        {
            SetValueToChanel(channelType.ToString(), value);
            PlayerPrefs.SetFloat(channelType.ToString(), value);
            PlayerPrefs.Save();
        }

        public void SetVolumeWithoutSave(SoundVolumeTypeId channelType, float value)
        {
            SetValueToChanel(channelType.ToString(), value);
        }

        public void NextSong()
        {
            PlayNextSong().Forget();
        }

        public void PreviousSong()
        {
            PlayPreviousSong().Forget();
        }

        public AudioClip GetCurrentMusicClip()
        {
            return _mainSoundContainer.MusicSource.clip;
        }

        public float GetVolumeForChannel(SoundVolumeTypeId channelType)
        {
            return GetSavedVolumeValueInternal(channelType);
        }

        #endregion

        #region Unity Events

        private void OnApplicationFocus(bool hasFocus)
        {
            if (_initialized == false)
                return;

            if (hasFocus)
            {
                ResetVolume();
            }
            else
            {
                MuteVolume();
            }
        }

        #endregion

        public void MuteVolume()
        {
            for (SoundVolumeTypeId i = 0; i < SoundVolumeTypeId.Count; i++)
                SetValueToChanel(i.ToString(), 0);
        }

        public void ResetVolume()
        {
            for (SoundVolumeTypeId i = 0; i < SoundVolumeTypeId.Count; i++)
                LoadVolume(i);
        }

        #region ILoadProgressStateStateHandler

        public OrderType StateHandlerOrder => OrderType.Last;

        public void OnEnterLoadProgress()
        {
            InitSoundService().Forget();
        }

        public void OnExitLoadProgress()
        {
        }

        public void OnEnterMainMenu()
        {
            PlayCurrentSong().Forget();
        }

        public void OnExitMainMenu()
        {
        }

        #endregion

        public void PlaySound(SoundTypeId typeId)
        {
            SoundConfig soundSetup = _staticDataService.Get<SoundsStaticData>().GetSoundConfig(typeId);

            if (soundSetup == null)
            {
                _loggerService.LogError($"Null sound setup for {typeId}!");
                return;
            }

            PlayNormal(soundSetup);
        }

        public void PlaySound(SoundTypeId typeId, AudioSource audioSource)
        {
            var staticData = _staticDataService.Get<SoundsStaticData>();
            SoundConfig soundSetup = staticData.GetSoundConfig(typeId);

            if (soundSetup == null)
            {
                _loggerService.LogError($"Null sound setup for {typeId}!");
                return;
            }

            PlayNormal(soundSetup, audioSource);
        }

        public void PlayOneShotSound(SoundTypeId soundTypeId, AudioSource audioSource)
        {
            SoundConfig config = GetSoundConfig(soundTypeId);
            SoundSetup setup = config.Data;
            audioSource.clip = GetClip(audioSource.clip, setup);
            SetupPitch(setup, audioSource);
            audioSource.PlayOneShot(audioSource.clip, volumeScale: setup.Volume);
        }

        public void PlayOneShotSound(SoundTypeId soundTypeId)
        {
            SoundConfig config = GetSoundConfig(soundTypeId);
            SoundSetup setup = config.Data;
            AudioSource soundSource = GetSourceForSoundType(setup);
            soundSource.clip = GetClip(soundSource.clip, setup);
            SetupPitch(setup, soundSource);
            soundSource.PlayOneShot(soundSource.clip, volumeScale: setup.Volume);
        }

        public void StopSound(SoundTypeId typeId)
        {
            SoundConfig clip = _staticDataService.Get<SoundsStaticData>().GetSoundConfig(typeId);
            StopClip(clip);
        }

        public void PlayMusic(SoundTypeId typeId)
        {
            PlayMusicAsync(typeId).Forget();
        }

        public void StopMusic()
        {
            _mainSoundContainer.MusicSource.Stop();
        }

        #region Private Methods

        private async UniTaskVoid InitSoundService()
        {
            _staticData = _staticDataService.Get<SoundsStaticData>();
            var go = await _assetProvider.LoadAssetAsync<GameObject>(MAIN_SOUND_CONTAINER);
            _mainSoundContainer = Instantiate(go, transform).GetComponent<MainSoundContainer>();

            ResetVolume();

            SoundConfig soundSetup = GetSoundConfig(SoundTypeId.GameplayMusic);

            int clipIndex = PlayerPrefs.HasKey(CurrentMusicClipIndex)
                ? Random.Range(0, soundSetup.Data.Clips.Length)
                : 0;

            PlayerPrefs.SetInt(CurrentMusicClipIndex, clipIndex);

            PlayCurrentSong().Forget();
            _initialized = true;
        }

        private void LoadVolume(SoundVolumeTypeId channelType)
        {
            float currentValue = GetSavedVolumeValueInternal(channelType);
            SetValueToChanel(channelType.ToString(), currentValue);
        }

        /// <summary>
        /// Mixer volume
        /// </summary>
        /// <param name="channelType"></param>
        /// <returns></returns>
        private static float GetCurrentVolumeValueInternal(SoundVolumeTypeId channelType)
        {
            float currentValue = GetSavedVolumeValueInternal(channelType);
            float volume = Mathf.Lerp(MIN_MIXER_VOLUME, MAX_MIXER_VOLUME, currentValue);
            return volume;
        }

        /// <summary>
        /// Volume from 0 to 1
        /// </summary>
        /// <param name="channelType"></param>
        /// <returns></returns>
        private static float GetSavedVolumeValueInternal(SoundVolumeTypeId channelType)
        {
            float currentValue = PlayerPrefs.GetFloat(channelType.ToString(), 0.75f);
            return currentValue;
        }

        private AudioClip GetClip(AudioClip currentClip, SoundSetup soundSetup)
        {
            if (soundSetup.Clips.Length <= 1)
                return soundSetup.Clips.FirstOrDefault();

            while (true)
            {
                var newClip = soundSetup.Clips[Random.Range(0, soundSetup.Clips.Length)];
                if (newClip == currentClip)
                    continue;
                currentClip = newClip;
                break;
            }

            return currentClip;
        }

        private void SetValueToChanel(string exposedParam, float value)
        {
            float volume = value > 0
                ? Mathf.Log10(value) * 20
                : -80f;

            _mainSoundContainer.Mixer.SetFloat(exposedParam, volume);
        }

        private AudioSource GetSourceForSoundType(SoundSetup soundSetup)
        {
            return soundSetup.SourceType.ToString() switch
            {
                nameof(SoundSourceTypeId.Music) => _mainSoundContainer.MusicSource,
                nameof(SoundSourceTypeId.CommonSfx) => GetAvailableAudioSource(soundSetup.SourceType),
                nameof(SoundSourceTypeId.RareSfx) => _mainSoundContainer.RareSFXSource,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private AudioSource GetAvailableAudioSource(SoundSourceTypeId typeId)
        {
            foreach (AudioSource source in _mainSoundContainer.SfxList)
            {
                if (source.isPlaying)
                    continue;

                return source;
            }

            return _mainSoundContainer.SfxList[0];
        }

        private void PlayNormal(SoundConfig soundConfig, AudioSource soundSource = null)
        {
            SoundSetup soundSetup = soundConfig.Data;
            soundSource ??= GetSourceForSoundType(soundSetup);

            soundSource.clip = GetClip(soundSource.clip, soundSetup);

            SetupPitch(soundSetup, soundSource);

            soundSource.volume = soundSetup.Volume;
            soundSource.Play();
        }

        private void StopClip(SoundConfig clip)
        {
            AudioSource soundSource = GetSourceForSoundType(clip.Data);
            soundSource.Stop();
        }

        private static void SetupPitch(SoundSetup soundSetup, AudioSource soundSource)
        {
            if (soundSetup.RandomizePitch)
            {
                float pitch = Random.Range(soundSetup.PitchMin, soundSetup.PitchMax);
                soundSource.pitch = pitch;
            }
            else
            {
                soundSource.pitch = 1;
            }
        }

        private async UniTask PlayNextSong()
        {
            IncreaseClipIndex();
            await PlayCurrentSong();
        }

        private async UniTask PlayPreviousSong()
        {
            DecreaseClipIndex();
            await PlayCurrentSong();
        }

        private async UniTask PlayCurrentSong()
        {
            AudioClip currentClip = GetCurrentMusicClipInternal();

            if (currentClip == null)
                return;

            if (currentClip == _mainSoundContainer.MusicSource.clip)
                return;

            await FadeToClip(_mainSoundContainer.MusicSource, currentClip);

            PlayGameplayMusic().Forget();
            OnSongUpdated?.Invoke();
        }

        private void IncreaseClipIndex()
        {
            SoundConfig soundSetup = GetSoundConfig(SoundTypeId.GameplayMusic);
            AudioClip[] musicClips = soundSetup.Data.Clips;

            int currentSongIndex = PlayerPrefs.GetInt(CurrentMusicClipIndex, -1);
            currentSongIndex++;

            if (currentSongIndex >= musicClips.Length)
                currentSongIndex = 0;

            PlayerPrefs.SetInt(CurrentMusicClipIndex, currentSongIndex);
            PlayerPrefs.Save();
        }

        private void DecreaseClipIndex()
        {
            SoundConfig soundSetup = GetSoundConfig(SoundTypeId.GameplayMusic);
            AudioClip[] musicClips = soundSetup.Data.Clips;

            int currentSongIndex = PlayerPrefs.GetInt(CurrentMusicClipIndex, 0);
            currentSongIndex--;

            if (currentSongIndex < 0)
                currentSongIndex = musicClips.Length - 1;

            PlayerPrefs.SetInt(CurrentMusicClipIndex, currentSongIndex);
            PlayerPrefs.Save();
        }

        private AudioClip GetCurrentMusicClipInternal()
        {
            int currentSongIndex = PlayerPrefs.GetInt(CurrentMusicClipIndex, 0);
            SoundConfig soundSetup = GetSoundConfig(SoundTypeId.GameplayMusic);
            AudioClip currentClip = soundSetup.Data.Clips[currentSongIndex];
            return currentClip;
        }

        private async UniTask PlayMusicAsync(SoundTypeId typeId)
        {
            SoundConfig soundSetup = _staticDataService.Get<SoundsStaticData>().GetSoundConfig(typeId);

            if (soundSetup == null)
            {
                PlayGameplayMusic().Forget();
                return;
            }

            ResetMusicToken();
            AudioClip audioClip = soundSetup.Data.Clips[Random.Range(0, soundSetup.Data.Clips.Length)];
            await FadeToClip(_mainSoundContainer.MusicSource, audioClip);
        }

        private async UniTask FadeToClip(AudioSource soundSource, AudioClip newClip)
        {
            soundSource.clip = newClip;
            soundSource.Play();
        }

        private async UniTaskVoid PlayGameplayMusic()
        {
            AudioSource soundSource = _mainSoundContainer.MusicSource;
            ResetMusicToken();

            while (_gamePlayMusicToken.Token.IsCancellationRequested == false)
            {
                await DelaySeconds(soundSource.clip.length, _gamePlayMusicToken.Token);
                await PlayNextSong();
            }
        }

        private void ResetMusicToken()
        {
            CancellationToken token = _mainSoundContainer.MusicSource.GetCancellationTokenOnDestroy();
            _gamePlayMusicToken?.Cancel();
            _gamePlayMusicToken = CancellationTokenSource.CreateLinkedTokenSource(token);
        }

        private SoundConfig GetSoundConfig(SoundTypeId soundTypeId)
        {
            return _staticData.GetSoundConfig(soundTypeId);
        }

        #endregion
    }
}