using Code.Gameplay.Sound.Service;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Sound.Behaviours
{
    public class Collision2DSoundComponent : MonoBehaviour
    {
        [SerializeField] private SoundTypeId _soundType = SoundTypeId.Unknown;
        [SerializeField] private SoundTypeId _overrideSound = SoundTypeId.Unknown;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _minSpeed = 1;
        [SerializeField] private Rigidbody2D _rb;

        private ISoundService _soundService;

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_rb.linearVelocity.magnitude < _minSpeed)
                return;
            
            if (_overrideSound is not SoundTypeId.Unknown)
            {
                _soundService.PlaySound(_overrideSound);
            }
            else if (_soundType is not SoundTypeId.Unknown)
            {
                if (_audioSource != null)
                {
                    _soundService.PlayOneShotSound(_soundType, _audioSource);
                }
                else
                {
                    _soundService.PlayOneShotSound(_soundType);
                }
            }
        }
    }
}