using System.Collections.Generic;
using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundState
{
    public class ProcessRoundCompleteWhenLootConsumedSystem : IExecuteSystem
    {
        private readonly IRoundStateService _roundStateService;
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _collectedLoot;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessRoundCompleteWhenLootConsumedSystem(GameContext context, IRoundStateService roundStateService)
        {
            _roundStateService = roundStateService;
            _roundController = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.RoundStateController,
                    GameMatcher.RoundOver
                ));

            _collectedLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.Collected
                ));
        }

        public void Execute()
        {
            foreach (var entity in _roundController.GetEntities(_buffer))
            {
                if (_collectedLoot.GetEntities().Length == 0)
                {
                    entity.isRoundComplete = true;
                    entity.isRoundOver = false;
                    _roundStateService.RoundComplete();
                }
            }
        }
    }
}