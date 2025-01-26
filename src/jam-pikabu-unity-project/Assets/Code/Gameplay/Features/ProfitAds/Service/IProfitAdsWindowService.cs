using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Features.ProfitAds.Service
{
    public interface IProfitAdsWindowService
    {
        UniTask TryShowProfitWindow();
    }
}