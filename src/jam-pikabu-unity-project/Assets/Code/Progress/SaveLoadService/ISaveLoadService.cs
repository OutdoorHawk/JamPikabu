namespace Code.Progress.SaveLoadService
{
    public interface ISaveLoadService
    {
        const string PROGRESS_KEY = "player_progress";
        void CreateProgress();
        void SaveProgress();
        void LoadProgress();
        bool HasSavedProgress { get; }
    }
}