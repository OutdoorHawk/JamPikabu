using Code.Meta.Features.BonusLevel.Service;
using Entitas;

namespace Code.Meta.Features.BonusLevel.Systems
{
    public class InitBonusLevelTimerSystem : IInitializeSystem
    {
        private readonly IGroup<MetaEntity> _timers;
        private readonly IBonusLevelService _bonusLevelService;

        public InitBonusLevelTimerSystem(MetaContext context, IBonusLevelService bonusLevelService)
        {
            _bonusLevelService = bonusLevelService;
            _timers = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.BonusLevelAvailableTime,
                MetaMatcher.BonusLevelAvailableTimer));
        }

        public void Initialize()
        {
            foreach (var entity in _timers)
            {
                _bonusLevelService.UpdateTimeToNextBonusLevel(entity.BonusLevelAvailableTime);
            }
        }
    }
}