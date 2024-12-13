using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class InitCurrentDaySystem : IInitializeSystem
    {
        private readonly IRoundStateService _roundStateService;
        private readonly IGroup<MetaEntity> _days;

        public InitCurrentDaySystem(MetaContext context, IRoundStateService roundStateService)
        {
            _roundStateService = roundStateService;
            _days = context.GetGroup(MetaMatcher.AllOf(MetaMatcher.Day));
        }

        public void Initialize()
        {
            foreach (var day in _days)
            {
                _roundStateService.SetCurrentDay(day.Day);
            }
        }
    }
}