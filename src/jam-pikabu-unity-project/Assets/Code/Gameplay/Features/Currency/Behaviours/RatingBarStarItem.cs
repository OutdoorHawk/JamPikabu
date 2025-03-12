using Code.Common.Extensions.Animations;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Meta.Features.Days.Configs.Stars;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class RatingBarStarItem : MonoBehaviour
    {
        public Animator StarAnimator;

        private ISoundService _soundService;

        private bool _replenishPlayed;
        public int RatingAmount { get; private set; }

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        public void Init(in DayStarData data)
        {
            RatingAmount = data.RatingAmountNeed;
        }

        public void PlayReplenish()
        {
            if (_replenishPlayed)
                return;

            StarAnimator.SetTrigger(AnimationParameter.Replenish.AsHash());
            _replenishPlayed = true;
            _soundService.PlaySound(SoundTypeId.StarReceive);
        }

        public void ResetReplenish()
        {
            if (_replenishPlayed == false)
                return;

            _replenishPlayed = false;
            StarAnimator.SetTrigger(AnimationParameter.Reset.AsHash());
            _soundService.PlaySound(SoundTypeId.StarLoose);
        }
    }
}