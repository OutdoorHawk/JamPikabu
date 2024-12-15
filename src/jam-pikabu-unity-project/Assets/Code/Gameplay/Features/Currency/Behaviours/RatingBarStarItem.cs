using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class RatingBarStarItem : MonoBehaviour
    {
        public Image Icon;
        public TMP_Text Amount;
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
            var currencyStaticData = _staticDataService.GetStaticData<CurrencyStaticData>();
            Icon.sprite = currencyStaticData.GetCurrencyConfig(CurrencyTypeId.Star).Data.Icon;
        }

        public void Init(in DayStarData data)
        {
            Amount.text = data.RatingAmountNeed.ToString();
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
            _replenishPlayed = false;
        }
    }
}