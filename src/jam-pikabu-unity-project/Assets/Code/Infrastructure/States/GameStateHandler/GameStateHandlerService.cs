﻿using System.Collections.Generic;
using Code.Infrastructure.States.GameStateHandler.Handlers;

namespace Code.Infrastructure.States.GameStateHandler
{
    public class GameStateHandlerService : IGameStateHandlerService
    {
        private readonly List<IEnterBootstrapStateHandler> _enterBootstrapStateHandlers;
        private readonly List<ILoadProgressStateHandler> _enterLoadProgressStateHandlers;
        private readonly List<IMainMenuStateHandler> _mainMenuStateHandlers;
        private readonly List<IEnterGameLoopStateHandler> _enterGameLoopStateHandlers;
        private readonly List<IExitGameLoopStateHandler> _exitGameLoopStateHandlers;
        private readonly List<ILoadLevelStateHandler> _loadLevelStateHandlers;

        public GameStateHandlerService
        (
            List<IEnterBootstrapStateHandler> enterBootstrapStateHandlers,
            List<ILoadProgressStateHandler> enterLoadProgressStateHandlers,
            List<IMainMenuStateHandler> mainMenuStateHandlers,
            List<IEnterGameLoopStateHandler> enterGameLoopStateHandlers,
            List<IExitGameLoopStateHandler> exitGameLoopStateHandlers,
            List<ILoadLevelStateHandler> loadLevelStateHandlers
        )
        {
            _enterBootstrapStateHandlers = enterBootstrapStateHandlers;
            _enterLoadProgressStateHandlers = enterLoadProgressStateHandlers;
            _enterGameLoopStateHandlers = enterGameLoopStateHandlers;
            _exitGameLoopStateHandlers = exitGameLoopStateHandlers;
            _loadLevelStateHandlers = loadLevelStateHandlers;
            _mainMenuStateHandlers = mainMenuStateHandlers;
            
            _enterBootstrapStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _enterLoadProgressStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _enterGameLoopStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _mainMenuStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _exitGameLoopStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _loadLevelStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
        }

        public void OnEnterBootstrapState()
        {
            foreach (var handler in _enterBootstrapStateHandlers) 
                handler.OnEnterBootstrap();
        }

        public void OnEnterLoadProgressState()
        {
            foreach (var handler in _enterLoadProgressStateHandlers) 
                handler.OnEnterLoadProgress();
        }

        public void OnExitLoadProgressState()
        {
            foreach (var handler in _enterLoadProgressStateHandlers) 
                handler.OnExitLoadProgress();
        }

        public void OnEnterMainMenu()
        {
            foreach (var handler in _mainMenuStateHandlers) 
                handler.OnEnterMainMenu();
        }
        
        public void OnExitMainMenu()
        {
            foreach (var handler in _mainMenuStateHandlers) 
                handler.OnExitMainMenu();
        }

        public void OnEnterLoadLevel()
        {
            foreach (var handler in _loadLevelStateHandlers) 
                handler.OnEnterLoadLevel();
        }
        
        public void OnExitLoadLevel()
        {
            foreach (var handler in _loadLevelStateHandlers) 
                handler.OnExitLoadLevel();
        }

        public void OnEnterGameLoop()
        {
            foreach (var handler in _enterGameLoopStateHandlers) 
                handler.OnEnterGameLoop();
        }

        public void OnExitGameLoop()
        {
            foreach (var handler in _exitGameLoopStateHandlers) 
                handler.OnExitGameLoop();
        }
    }
}