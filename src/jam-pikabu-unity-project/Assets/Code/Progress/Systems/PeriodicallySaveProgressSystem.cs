using Code.Gameplay.Common.Time;
using Code.Infrastructure.Common.Time;
using Code.Infrastructure.Systems;
using Code.Progress.SaveLoadService;

namespace Code.Progress.Systems
{
    public class PeriodicallySaveProgressSystem : TimerExecuteSystem
    {
        private readonly ISaveLoadService _saveLoadService;

        public PeriodicallySaveProgressSystem(float executeIntervalSeconds, ITimeService time,
            ISaveLoadService saveLoadService) : base(executeIntervalSeconds, time)
        {
            _saveLoadService = saveLoadService;
        }

        protected override void Execute()
        {
            _saveLoadService.SaveProgress();
        }
    }
}