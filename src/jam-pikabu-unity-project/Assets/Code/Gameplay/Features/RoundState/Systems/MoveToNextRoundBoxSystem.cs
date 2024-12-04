using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class MoveToNextRoundBoxSystem : IExecuteSystem
    {
        private readonly IRoundStateService _roundStateService;
        private readonly IGroup<GameEntity> _roundController;
        private readonly IGroup<GameEntity> _loot;

        public MoveToNextRoundBoxSystem(GameContext context, IRoundStateService roundStateService)
        {
            _roundStateService = roundStateService;
            _roundController = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundOver
                ));

            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Collected,
                    GameMatcher.Loot
                ));
        }

        public void Execute()
        {
            foreach (var controller in _roundController)
            {
                if (LootIsApplying())
                    continue;

                _roundStateService.TryLoadNextLevel();
            }
        }
        
        private bool LootIsApplying()
        {
            return _loot.GetEntities().Length > 0;
        }
    }
}