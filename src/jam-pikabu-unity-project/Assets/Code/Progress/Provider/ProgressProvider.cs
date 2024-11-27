using Code.Progress.Data;

namespace Code.Progress.Provider
{
    public class ProgressProvider : IProgressProvider
    {
        public PlayerProgress Progress { get; private set; }
        public EntityData EntityData => Progress.EntityData;
        
        public void SetProgressData(PlayerProgress data)
        {
            Progress = data;
        }
    }
}