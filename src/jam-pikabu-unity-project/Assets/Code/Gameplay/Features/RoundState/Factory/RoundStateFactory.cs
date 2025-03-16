using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Infrastructure.ABTesting;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;

namespace Code.Gameplay.Features.RoundState.Factory
{
    public class RoundStateFactory : IRoundStateFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;
        private readonly IABTestService _abTestService;

        public RoundStateFactory
        (
            IStaticDataService staticDataService,
            IDaysService daysService,
            IABTestService abTestService
        )
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
            _abTestService = abTestService;
        }

        public GameEntity CreateRoundStateController()
        {
            GameEntity controller = CreateGameEntity
                .Empty()
                .With(x => x.isRoundStateController = true)
                .With(x => x.isRoundStartAvailable = true);

            AddControllerComponents(controller);
            return controller;
        }

        private void AddControllerComponents(GameEntity roundStateController)
        {
            switch (_abTestService.GetExperimentValue(ExperimentTagTypeId.TIMER_REPLACE))
            {
                case ExperimentValueTypeId.@default:
                    CreateDefaultController(roundStateController);
                    break;
                case ExperimentValueTypeId.replace_timer_with_attempts:
                    CreateRoundAttemptsController(roundStateController);
                    break;
            }
        }

        private void CreateDefaultController(GameEntity roundStateController)
        {
            if (CheckBonusLevelDuration(roundStateController))
                return;

            var daysStaticData = _staticDataService.Get<DaysStaticData>();

            if (_daysService.CurrentDay >= daysStaticData.DayToStartTimer == false)
                return;

            float roundDuration = _daysService.GetDayData().IsBossDay
                ? daysStaticData.BossRoundDuration
                : daysStaticData.DefaultRoundDuration;

            roundStateController.AddRoundDuration(roundDuration);
            roundStateController.AddRoundTimeLeft(roundDuration);
        }

        private void CreateRoundAttemptsController(GameEntity roundStateController)
        {
            if (CheckBonusLevelDuration(roundStateController))
                return;

            var daysStaticData = _staticDataService.Get<DaysStaticData>();

            int attempts = _daysService.GetDayData().IsBossDay
                ? daysStaticData.BossRoundHookAttempts
                : daysStaticData.DefaultRoundHookAttempts;

            if (_daysService.CurrentDay >= daysStaticData.DayToStartTimer == false)
                return;

            roundStateController
                .AddHookAttemptsMax(attempts)
                .AddHookAttemptsLeft(attempts)
                ;
        }

        private bool CheckBonusLevelDuration(GameEntity roundStateController)
        {
            if (_daysService.BonusLevelType is BonusLevelType.GoldenCoins)
            {
                int roundTimeOverride = _daysService.BonusLevelData.RoundTimeOverride;
                roundStateController.AddRoundDuration(roundTimeOverride);
                roundStateController.AddRoundTimeLeft(roundTimeOverride);
                return true;
            }

            return false;
        }
    }
}