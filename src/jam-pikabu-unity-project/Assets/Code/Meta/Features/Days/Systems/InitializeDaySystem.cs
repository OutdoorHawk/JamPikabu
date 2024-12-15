using System.Collections.Generic;
using System.Linq;
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
            MetaEntity[] metaEntities = _days.GetEntities();
            IEnumerable<DayProgressData> dayProgressData = metaEntities.Select(x => new DayProgressData(x.Day, x.StarsAmount));
            _daysService.InitializeDays(dayProgressData);
        }
    }
}