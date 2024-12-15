using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
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

        public ApplyDayProgressOnEndDay(GameContext context, MetaContext meta,
            ISaveLoadService saveLoadService, IDaysService daysService) : base(context)
        {
            _saveLoadService = saveLoadService;
            _daysService = daysService;

            _days = meta.GetGroup(MetaMatcher.Day);
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
            MetaEntity day = TryFindExistingDay();

            if (day != null)
            {
                day.ReplaceStarsAmount(0); //TODO: STAR COUNT
            }
            else
            {
                CreateMetaEntity.Empty()
                    .With(x => x.AddDay(_daysService.CurrentDay))
                    .ReplaceStarsAmount(0);
            }

            _saveLoadService.SaveProgress();
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
    }
}