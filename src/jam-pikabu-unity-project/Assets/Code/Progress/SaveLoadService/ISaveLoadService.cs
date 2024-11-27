namespace Code.Progress.SaveLoadService
{
    public interface ISaveLoadService
    {
        void CreateProgress();
        void SaveProgress();
        void LoadProgress();
        bool HasSavedProgress { get; }
    }
}