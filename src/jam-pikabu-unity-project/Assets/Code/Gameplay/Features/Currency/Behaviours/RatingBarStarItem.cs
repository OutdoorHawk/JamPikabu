using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Configs.Stars;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class RatingBarStarItem : MonoBehaviour
    {
        public Image StarEmpty;
        public Image StarFull;
        public Animator StarAnimator;

        private IStaticDataService _staticDataService;

        private bool _replenishPlayed;
        public int RatingAmount { get; private set; }

        [Inject]
        private void Construct(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        private void Start()
        {

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
        }

        public void ResetReplenish()
        {
            if (_replenishPlayed == false)
                return;
            
            _replenishPlayed = false;
            StarAnimator.SetTrigger(AnimationParameter.Reset.AsHash());
        }
    }
}