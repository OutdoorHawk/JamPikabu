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
            IEnumerable<DayProgressData> dayProgressData = metaEntities.Select(CreateProgressData);
            _daysService.InitializeDays(dayProgressData);
        }

        private DayProgressData CreateProgressData(MetaEntity x)
        {
            if (x.hasStarsAmountSeen == false) 
                x.AddStarsAmountSeen(x.StarsAmount);
            
            var data = new DayProgressData(x.Day, x.StarsAmount, x.StarsAmountSeen);
            return data;
        }
    }
}