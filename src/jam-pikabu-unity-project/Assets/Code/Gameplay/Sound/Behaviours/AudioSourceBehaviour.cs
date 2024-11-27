using Code.Gameplay.Sound.Service;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Sound.Behaviours
{
    public class AudioSourceBehaviour : MonoBehaviour
    {
        [SerializeField] private SoundTypeId attackType;
        [SerializeField] private SoundTypeId stepType;
        [SerializeField] private SoundTypeId deathTypeId;
        [SerializeField] private SoundTypeId takeDamageTypeId;
        [SerializeField] private SoundTypeId abilityTypeId;

        [SerializeField] private AudioSource attackSource;
        [SerializeField] private AudioSource footStepSource;
        [SerializeField] private AudioSource deathSource;
        [SerializeField] private AudioSource takeDamageSource;
        [SerializeField] private AudioSource abilitySource;

        private ISoundService _soundService;

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        public void PlayAttack()
        {
            if (attackSource != null && attackType != SoundTypeId.Unknown)
            {
                _soundService.PlaySound(attackType, attackSource);
            }
        }

        public void PlayFootStep()
        {
            if (footStepSource != null && stepType != SoundTypeId.Unknown)
            {
                _soundService.PlaySound(stepType, footStepSource);
            }
        }

        public void PlayDeath()
        {
            if (deathSource != null && deathTypeId != SoundTypeId.Unknown)
            {
                _soundService.PlaySound(deathTypeId, deathSource);
            }
        }

        public void PlayTakeDamage()
        {
            if (takeDamageSource != null && takeDamageTypeId != SoundTypeId.Unknown)
            {
                _soundService.PlaySound(takeDamageTypeId, takeDamageSource);
            }
        }

        public void PlayAbility()
        {
            if (abilitySource != null && abilityTypeId != SoundTypeId.Unknown)
            {
                _soundService.PlaySound(abilityTypeId, abilitySource);
            }
        }
    }
}