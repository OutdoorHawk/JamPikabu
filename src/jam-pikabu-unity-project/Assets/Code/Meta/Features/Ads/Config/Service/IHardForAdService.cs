namespace Code.Meta.Features.Ads.Config.Service
{
    public interface IHardForAdService
    {
        int GetCurrentBonusAmount();
        void GiveHardForAdReward();
    }
}