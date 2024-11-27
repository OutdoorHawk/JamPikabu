using System;
using Code.Common.Ads.Handler;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Infrastructure.Ads.Service;

namespace Code.Meta.UI.HardCurrencyHolder.Service
{
    public class StorageUIService : IStorageUIService, IAdsSuccsessfulHandler
    {
        private readonly ISoundService _soundService;
        private readonly IAdsService _adsService;

        private float _currentHard;
        private bool _canPlaySoundForce;

        public event Action HardChanged;

        public float CurrentHard => _currentHard;

        public StorageUIService(ISoundService soundService, IAdsService adsService)
        {
            _adsService = adsService;
            _soundService = soundService;
            _adsService.RegisterAdsHandler(this);
        }

        public void OnAdsSuccsessful()
        {
            _canPlaySoundForce = true;
        }

        public void UpdateCurrentHard(float newAmount)
        {
            if (Math.Abs(newAmount - _currentHard) > float.Epsilon)
            {
                PlayHardSound(newAmount);
                _currentHard = newAmount;
                HardChanged?.Invoke();
            }
        }

        private void PlayHardSound(float newAmount)
        {
            if (_currentHard == 0 && _canPlaySoundForce == false)
                return;
            
            _canPlaySoundForce = false;
            if (newAmount > _currentHard)
            {
                _soundService.PlaySound(SoundTypeId.Hard_Currency_Collect);
                return;
            }

            if (newAmount < _currentHard)
            {
                _soundService.PlaySound(SoundTypeId.Hard_Currency_Spend);
                return;
            }
        }

        public void Cleanup()
        {
            _currentHard = 0f;
            HardChanged = null;
        }
    }
}