using System.Collections.Generic;
using System.Linq;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Configs.Stars;
using Code.Meta.Features.Days.Service;
using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Gameplay.Features.Days.Systems
{
    public class ApplyDayProgressOnEndDay : ReactiveSystem<GameEntity>
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IDaysService _daysService;
        private readonly IGroup<MetaEntity> _days;
        private readonly IGroup<GameEntity> _ratingPlus;
        private readonly IGroup<GameEntity> _ratingMinus;

        public ApplyDayProgressOnEndDay(GameContext context, MetaContext meta,
            ISaveLoadService saveLoadService, IDaysService daysService) : base(context)
        {
            _saveLoadService = saveLoadService;
            _daysService = daysService;

            _days = meta.GetGroup(MetaMatcher.Day);
            _ratingPlus = context.GetGroup(GameMatcher.AllOf(GameMatcher.CurrencyStorage, GameMatcher.Plus));
            _ratingMinus = context.GetGroup(GameMatcher.AllOf(GameMatcher.CurrencyStorage, GameMatcher.Minus));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.GameState,
                GameMatcher.EndDay).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isGameState && entity.isEndDay;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            int starsReceived = GetStarsReceived();
            if (starsReceived != 0)
            {
                MetaEntity day = TryFindExistingDay() ?? CreateNewDayProgressEntity();

                UpdateStarsAmount(day, starsReceived);

                _daysService.StarsRecieved(starsReceived);
                _saveLoadService.SaveProgress();
            }
            
            _daysService.DayComplete();
        }

        private int GetStarsReceived()
        {
            var starsData = _daysService.DayStarsData;
            int totalRating = _ratingPlus.GetEntities().Sum(x => x.Plus) - _ratingMinus.GetEntities().Sum(x => x.Minus);
            int starsReceived = starsData.Count(starData => totalRating >= starData.RatingAmountNeed);
            return starsReceived;
        }

        private MetaEntity TryFindExistingDay()
        {
            foreach (MetaEntity day in _days)
            {
                if (day.Day != _daysService.CurrentDay)
                    continue;

                return day;
            }

            return null;
        }

        private MetaEntity CreateNewDayProgressEntity()
        {
            return CreateMetaEntity.Empty()
                .With(x => x.AddDay(_daysService.CurrentDay))
                .AddStarsAmount(0)
                .AddStarsAmountSeen(0)
                ;
        }

        private void UpdateStarsAmount(MetaEntity day, int starsReceived)
        {
            if (starsReceived > day.StarsAmount)
                day.ReplaceStarsAmount(starsReceived);
        }
    }
}