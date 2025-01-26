using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.ProfitAds.Window;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Ads.Config;
using Code.Infrastructure.Ads.Service;
using Code.Meta.Features.Days.Service;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Features.ProfitAds.Service
{
    public class ProfitAdsWindowService : IProfitAdsWindowService
    {
        private readonly IAdsService _adsService;
        private readonly IWindowService _windowService;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IStaticDataService _staticDataService;
        private readonly ITimeService _timeService;
        private readonly IDaysService _daysService;

        private int _lastTimeShowed = 0;

        private AdsStaticData AdsStaticData => _staticDataService.Get<AdsStaticData>();

        public ProfitAdsWindowService
        (
            IAdsService adsService,
            IWindowService windowService,
            IGameplayCurrencyService gameplayCurrencyService,
            IStaticDataService staticDataService,
            ITimeService timeService,
            IDaysService daysService
        )
        {
            _adsService = adsService;
            _windowService = windowService;
            _gameplayCurrencyService = gameplayCurrencyService;
            _staticDataService = staticDataService;
            _timeService = timeService;
            _daysService = daysService;
        }

        public async UniTask TryShowProfitWindow()
        {
            if (_adsService.CanShowRewarded == false)
                return;

            if (_gameplayCurrencyService.CollectedGoldInLevel < AdsStaticData.DoubleProfitMinGold)
                return;

            if (_adsService.CanShowInterstitial)
                return;

            if (_daysService.GetDaysProgress().Count < AdsStaticData.LevelsPassedToStartProfitAds)
                return;

            if (CheckTimeDiff() == false)
                return;

            _lastTimeShowed = _timeService.TimeStamp;
            var window = await _windowService.OpenWindow<ProfitAdsWindow>(WindowTypeId.ProfitAdsWindow);
            await UniTask.WaitWhile(() => _windowService.IsWindowOpen(WindowTypeId.ProfitAdsWindow), cancellationToken: window.destroyCancellationToken);
        }

        private bool CheckTimeDiff()
        {
            if (_lastTimeShowed == 0)
                return true;

            int diff = _timeService.TimeStamp - _lastTimeShowed;

            if (diff < AdsStaticData.DoubleProfitIntervalSeconds)
                return false;

            return true;
        }
    }
}