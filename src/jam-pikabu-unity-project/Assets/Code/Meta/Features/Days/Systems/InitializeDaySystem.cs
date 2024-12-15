using Code.Meta.Features.Days.Service;
using Entitas;

namespace Code.Meta.Features.Days.Systems
{
    public class InitializeDaySystem : IInitializeSystem
    {
        private readonly IDaysService _daysService;
        private readonly IGroup<MetaEntity> _days;

        public InitializeDaySystem(MetaContext context, IDaysService daysService)
        {
            _daysService = daysService;
            _days = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Day));
        }

        public void Initialize()
        {
            if (_days.count == 0)
            {
                _daysService.SetCurrentDay(1);
            }
        }
    }
}