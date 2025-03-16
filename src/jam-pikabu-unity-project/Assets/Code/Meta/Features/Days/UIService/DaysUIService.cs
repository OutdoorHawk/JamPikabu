using Code.Gameplay.StaticData;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Days.Service;

namespace Code.Meta.Features.Days.UIService
{
    public class DaysUIService : IDaysUIService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;

        public DaysUIService
        (
            IStaticDataService staticDataService,
            IDaysService daysService
        )
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
        }

        public bool CheckLevelHasStars()
        {
            if (_daysService.DayStarsData.Count == 0)
                return false;

            if (_daysService.BonusLevelType == BonusLevelType.GoldenCoins)
                return false;

            return true;
        }
    }
}