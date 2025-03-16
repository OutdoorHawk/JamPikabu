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
            float roundDuration = GetRoundDuration();
            roundStateController.AddRoundDuration(roundDuration);
        }

        private void CreateRoundAttemptsController(GameEntity roundStateController)
        {
            int attempts = GetRoundAttempts();

            roundStateController
                .AddHookAttemptsMax(attempts)
                ;
        }

        private int GetRoundDuration()
        {
            var daysStaticData = _staticDataService.Get<DaysStaticData>();

            if (_daysService.BonusLevelType is BonusLevelType.GoldenCoins)
                return _daysService.BonusLevelData.RoundTimeOverride;

            if (_daysService.CanShowTimer() == false)
                return (int)daysStaticData.DisabledTimerRoundDuration;

            float roundDuration = _daysService.GetDayData().IsBossDay
                ? daysStaticData.BossRoundDuration
                : daysStaticData.DefaultRoundDuration;

            return (int)roundDuration;
        }

        private int GetRoundAttempts()
        {
            var daysStaticData = _staticDataService.Get<DaysStaticData>();
            return daysStaticData.DefaultRoundHookAttempts;
        }
    }
}