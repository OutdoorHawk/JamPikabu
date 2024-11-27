using Code.Progress.Data;

namespace Code.Progress.Provider
{
    public interface IProgressProvider
    {
        PlayerProgress Progress { get; }
        EntityData EntityData { get; }
        void SetProgressData(PlayerProgress data);
    }
}