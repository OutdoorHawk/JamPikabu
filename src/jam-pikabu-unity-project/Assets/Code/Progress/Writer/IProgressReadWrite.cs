namespace Code.Progress.Writer
{
    public interface IProgressReadWrite
    {
        bool HasSavedProgress();
        void WriteProgress(string json);
        T ReadProgress<T>() where T : class;
        void DeleteProgress();
    }
}