using System.Collections.Generic;
using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessNextRoundOrGameOverSystem : ReactiveSystem<GameEntity>
    {
        private readonly IRoundStateService _roundStateService;
        private readonly IGroup<GameEntity> _gold;

        public ProcessNextRoundOrGameOverSystem(GameContext context, IRoundStateService roundStateService) : base(context)
        {
            _roundStateService = roundStateService;
            _gold = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.CurrencyStorage,
                    GameMatcher.Gold
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.RoundStateController, 
                GameMatcher.RoundComplete));
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> rouncController)
        {
            foreach (var entity in rouncController)
            foreach (var gold in _gold)
            {
                if (entity.RoundCost > gold.Gold)
                {
                    _roundStateService.GameOver();
                }
                else
                {
                  //  _roundStateService.TryLoadNextLevel();
                }
            }
        }
    }
}