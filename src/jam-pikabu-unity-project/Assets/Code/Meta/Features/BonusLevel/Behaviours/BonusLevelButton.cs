using System;
using Code.Common.Ads.Handler;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time.Behaviours;
using Code.Infrastructure.Ads.Service;
using Code.Meta.Features.BonusLevel.Service;
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
        
        private IAdsService _adsService;
        private IBonusLevelService _bonusLevelService;

        [Inject]
        private void Construct(IAdsService adsService, IBonusLevelService bonusLevelService)
        {
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
            
            if (_adsService.CanShowRewarded == false)
            {
                Button.interactable = false;
                return;
            }
            
            if (_bonusLevelService.CanPlayBonusLevel() == false)
            {
                Button.interactable = false;
                StartTimer();
                return;
            }

            Timer.TimerText.text = "Бонусный уровень";
            Pin.EnableElement();
            Button.interactable = true;
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