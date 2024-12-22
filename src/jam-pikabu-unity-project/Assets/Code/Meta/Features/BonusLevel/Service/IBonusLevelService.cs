namespace Code.Meta.Features.BonusLevel.Service
{
    public interface IBonusLevelService
    {
        bool CanPlayFree();
        int GetTimeToBonusLevel();
        void UpdateTimeToNextBonusLevel(int bonusLevelAvailableTimeStamp);
        void LoadBonusLevel();
    }
}