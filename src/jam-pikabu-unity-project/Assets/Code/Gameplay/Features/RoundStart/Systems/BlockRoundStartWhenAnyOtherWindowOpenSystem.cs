using System.Collections.Generic;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundStart.Systems
{
    public class BlockRoundStartWhenAnyOtherWindowOpenSystem : IExecuteSystem
    {
        private readonly IWindowService _windowService;
        private readonly IGroup<GameEntity> _roundController;

        private readonly List<GameEntity> _buffer = new();

        public BlockRoundStartWhenAnyOtherWindowOpenSystem(GameContext gameContext, IWindowService windowService)
        {
            _windowService = windowService;

            _roundController = gameContext.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundStartAvailable
                ));
        }

        public void Execute()
        {
            foreach (var hook in _roundController.GetEntities(_buffer))
            foreach (var type in _windowService.Windows.Keys)
            {
                if (type is WindowTypeId.Cheats || type is WindowTypeId.PlayerHUD)
                    continue;

                hook.isRoundStartAvailable = false;
            }
        }
    }
}