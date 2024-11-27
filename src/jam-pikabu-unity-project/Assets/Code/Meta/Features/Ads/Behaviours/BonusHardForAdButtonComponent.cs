using Code.Gameplay.StaticData;
using Code.Infrastructure.Ads.Behaviours;
using Code.Infrastructure.Localization;
using Code.Meta.Features.Ads.Config.Service;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.Ads.Behaviours
{
    public class BonusHardForAdButtonComponent : MonoBehaviour, ILocalizationHandler
    {
        [SerializeField] private AdsButton _adsButton;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Image _currencyIcon;
        
        private IHardForAdService _hardForAdService;
        private ILocalizationService _localizationService;
        private IStaticDataService _staticDataService;

        [Inject]
        private void Construct
        (
            IHardForAdService hardForAdService,
            ILocalizationService localizationService,
            IStaticDataService staticDataService
        )
        {
            _staticDataService = staticDataService;
            _localizationService = localizationService;
            _hardForAdService = hardForAdService;
        }
        
        private void Start()
        {
            _adsButton.OnRewarded += ApplyReward;
            _localizationService.RegisterHandler(this);
            InitText();
           // _currencyIcon.sprite = _staticDataService.GetCurrencyConfig(CurrencyTypeId.Hard).Data.Icon;
        }

        private void OnDestroy()
        {
            _adsButton.OnRewarded -= ApplyReward;
            _localizationService.UnregisterHandler(this);
        }

        public void OnLanguageChanged(Locale locale)
        {
            InitText();
        }

        private void InitText()
        {
            int currentBonusAmount = _hardForAdService.GetCurrentBonusAmount();
            _amountText.text = $"+{currentBonusAmount}";
        }

        private void ApplyReward()
        {
            _hardForAdService.GiveHardForAdReward();
        }
    }
}