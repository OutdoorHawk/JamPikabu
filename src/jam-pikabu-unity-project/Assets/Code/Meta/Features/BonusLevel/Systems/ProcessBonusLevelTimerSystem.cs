using Code.Gameplay.Common.Time;
using Code.Meta.Features.BonusLevel.Service;
using Entitas;

namespace Code.Meta.Features.BonusLevel.Systems
{
    public class ProcessBonusLevelTimerSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _timers;
        private readonly IBonusLevelService _bonusLevelService;
        private readonly ITimeService _timeService;

        public ProcessBonusLevelTimerSystem(MetaContext context, IBonusLevelService bonusLevelService, ITimeService timeService)
        {
            _bonusLevelService = bonusLevelService;
            _timeService = timeService;
            _timers = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.BonusLevelAvailableTime,
                MetaMatcher.BonusLevelAvailableTimer));
        }

        public void Execute()
        {
            foreach (var entity in _timers)
            {
                if (_timeService.TimeStamp > entity.BonusLevelAvailableTime)
                {
                    entity.isDestructed = true;
                    _bonusLevelService.UpdateTimeToNextBonusLevel(0);
                }
            }
        }
    }
}