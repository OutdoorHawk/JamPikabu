using System.Collections.Generic;
using Code.Infrastructure.States.GameStateHandler.Handlers;

namespace Code.Infrastructure.States.GameStateHandler
{
    public class GameStateHandlerService : IGameStateHandlerService
    {
        private readonly List<ILoadProgressStateHandler> _enterLoadProgressStateHandlers;
        private readonly List<IMainMenuStateHandler> _mainMenuStateHandlers;
        private readonly List<IEnterGameLoopStateHandler> _enterGameLoopStateHandlers;
        private readonly List<IExitGameLoopStateHandler> _exitGameLoopStateHandlers;
        private readonly List<ILoadLevelStateHandler> _loadLevelStateHandlers;

        public GameStateHandlerService
        (
            List<ILoadProgressStateHandler> enterLoadProgressStateHandlers,
            List<IMainMenuStateHandler> mainMenuStateHandlers,
            List<IEnterGameLoopStateHandler> enterGameLoopStateHandlers,
            List<IExitGameLoopStateHandler> exitGameLoopStateHandlers,
            List<ILoadLevelStateHandler> loadLevelStateHandlers
        )
        {
            _enterLoadProgressStateHandlers = enterLoadProgressStateHandlers;
            _enterGameLoopStateHandlers = enterGameLoopStateHandlers;
            _exitGameLoopStateHandlers = exitGameLoopStateHandlers;
            _loadLevelStateHandlers = loadLevelStateHandlers;
            _mainMenuStateHandlers = mainMenuStateHandlers;
            
            _enterLoadProgressStateHandlers.Sort((x, y) => x.StateHandlerOrder.CompareTo(y.StateHandlerOrder));
            _enterGameLoopStateHandlers.Sort((x, y) => x.StateHandlerOrder.CompareTo(y.StateHandlerOrder));
            _mainMenuStateHandlers.Sort((x, y) => x.StateHandlerOrder.CompareTo(y.StateHandlerOrder));
            _exitGameLoopStateHandlers.Sort((x, y) => x.StateHandlerOrder.CompareTo(y.StateHandlerOrder));
            _loadLevelStateHandlers.Sort((x, y) => x.StateHandlerOrder.CompareTo(y.StateHandlerOrder));
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