namespace Code.Meta.Features.BonusLevel.Service
{
    public interface IBonusLevelService
    {
        bool CanPlayBonusLevel();
        int GetTimeToBonusLevel();
        void UpdateTimeToNextBonusLevel(int bonusLevelAvailableTimeStamp);
        void LoadBonusLevel();
    }
}