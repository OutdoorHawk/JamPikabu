using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.ProfitAds.Window;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Ads.Service;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Features.ProfitAds.Service
{
    public class ProfitAdsWindowService : IProfitAdsWindowService
    {
        private readonly IAdsService _adsService;
        private readonly IWindowService _windowService;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;

        public ProfitAdsWindowService
        (
            IAdsService adsService,
            IWindowService windowService,
            IGameplayCurrencyService gameplayCurrencyService
        )
        {
            _adsService = adsService;
            _windowService = windowService;
            _gameplayCurrencyService = gameplayCurrencyService;
        }

        public async UniTask TryShowProfitWindow()
        {
            if (_adsService.CanShowRewarded == false)
                return;
            
            if (_gameplayCurrencyService.CollectedGoldInLevel < 0)
                return;

            var window = await _windowService.OpenWindow<ProfitAdsWindow>(WindowTypeId.ProfitAdsWindow);
            await UniTask.WaitWhile(() => _windowService.IsWindowOpen(WindowTypeId.ProfitAdsWindow), cancellationToken: window.destroyCancellationToken);
        }
    }
}