using System;
using Code.Common.Ads.Handler;
using Code.Infrastructure.Ads.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Infrastructure.Ads.Behaviours
{
    [RequireComponent(typeof(Button))]
    public class AdsButton : MonoBehaviour, IAdsSuccsessfulHandler, IAdsErrorHandler
    {
        public event Action OnRewarded;
        public event Action<string> OnError;
        
        private Button _button;
            
        private IAdsService _adsService;

        [Inject]
        private void Construct(IAdsService adsService)
        {
            _adsService = adsService;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(TryWatchAd);
        }

        private void OnDestroy()
        {
            _adsService.UnregisterAdsHandler(this);
            _button.onClick.RemoveListener(TryWatchAd);
        }

        private void TryWatchAd()
        {
            _adsService.RegisterAdsHandler(this);
            _adsService.RequestRewardedAd();
        }

        void IAdsSuccsessfulHandler.OnAdsSuccsessful()
        {
            _adsService.UnregisterAdsHandler(this);
            OnRewarded?.Invoke();
        }

        void IAdsErrorHandler.OnAdsError(string error)
        {
            _adsService.UnregisterAdsHandler(this);
            OnError?.Invoke(error);
        }
    }
}