using System.Collections.Generic;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class BlockGrapplingHookMovementWhenAnyOtherWindowOpenSystem : IExecuteSystem
    {
        private readonly IWindowService _windowService;
        private readonly IGroup<GameEntity> _hooks;

        private readonly List<GameEntity> _buffer = new();

        public BlockGrapplingHookMovementWhenAnyOtherWindowOpenSystem(GameContext gameContext, IWindowService windowService)
        {
            _windowService = windowService;

            _hooks = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook)
                .AnyOf(GameMatcher.XAxisMovementAvailable,
                    GameMatcher.DescentAvailable));
        }

        public void Execute()
        {
            foreach (var hook in _hooks.GetEntities(_buffer))
            foreach (var type in _windowService.Windows.Keys)
            {
                if (type is WindowTypeId.Cheats || type is WindowTypeId.PlayerHUD)
                    continue;

                hook.isXAxisMovementAvailable = false;
                hook.isDescentAvailable = false;
            }
        }
    }
}