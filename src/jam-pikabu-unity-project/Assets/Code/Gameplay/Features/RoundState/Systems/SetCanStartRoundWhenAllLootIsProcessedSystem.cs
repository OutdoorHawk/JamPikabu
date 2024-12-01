using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class SetCanStartRoundWhenAllLootIsProcessedSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _loot;
        private readonly IGroup<GameEntity> _busyHook;
        private readonly List<GameEntity> _buffer = new(2);

        public SetCanStartRoundWhenAllLootIsProcessedSystem(GameContext context)
        {
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundOver
                ));
            
            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected
                ));
        }

        public void Execute()
        {
            foreach (var controller in _roundController.GetEntities(_buffer))
            {
                if (_loot.GetEntities().Length > 0)
                    continue;

                controller.isRoundOver = false;
                controller.isRoundStartAvailable = true;
            }
        }

      
    }
}