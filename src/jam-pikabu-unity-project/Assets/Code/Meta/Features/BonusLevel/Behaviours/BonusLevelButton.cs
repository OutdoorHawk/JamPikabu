using System;
using Code.Common.Ads.Handler;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time.Behaviours;
using Code.Infrastructure.Ads.Service;
using Code.Infrastructure.Localization;
using Code.Meta.Features.BonusLevel.Service;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.BonusLevel.Behaviours
{
    public class BonusLevelButton : MonoBehaviour, IAdsStartedHandler, IAdsSuccsessfulHandler, IAdsErrorHandler
    {
        public Button Button;
        public UniversalTimer Timer;
        public GameObject Pin;
        public UIShiny Shiny;
        
        private IAdsService _adsService;
        private IBonusLevelService _bonusLevelService;
        private ILocalizationService _localizationService;

        [Inject]
        private void Construct(IAdsService adsService, IBonusLevelService bonusLevelService, ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _bonusLevelService = bonusLevelService;
            _adsService = adsService;
        }

        private void Awake()
        {
            Button.onClick.AddListener(AskAd);
        }

        private void Start()
        {
            RefreshCanShowAd();
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(AskAd);
            Cleanup();
        }

        public void OnAdsStarted()
        {
            Button.interactable = false;
        }

        public void OnAdsSuccsessful()
        {
            Cleanup();
            LoadBonusLevel();
        }

        public void OnAdsError(string error)
        {
            Cleanup();
        }

        private void RefreshCanShowAd()
        {
            Pin.DisableElement();
            Shiny.Stop();
            Button.interactable = false;
            
            if (_adsService.CanShowRewarded == false)
            {
                return;
            }
            
            if (_bonusLevelService.CanPlayBonusLevel() == false)
            {
                StartTimer();
                return;
            }

            Timer.TimerText.text = _localizationService["MAIN MENU/BONUS_LEVEL"];
            Pin.EnableElement();
            Button.interactable = true;
            Shiny.Play();
        }

        private void AskAd()
        {
            _adsService.RegisterAdsHandler(this);
            _adsService.RequestRewardedAd();
        }

        private void StartTimer()
        {
           Func<int> getTime = _bonusLevelService.GetTimeToBonusLevel;
           Timer.StartTimer(getTime, RefreshCanShowAd);
        }

        private void LoadBonusLevel()
        {
            _bonusLevelService.LoadBonusLevel();
        }

        private void Cleanup()
        {
            Button.interactable = true;
            _adsService.UnregisterAdsHandler(this);
        }
    }
}