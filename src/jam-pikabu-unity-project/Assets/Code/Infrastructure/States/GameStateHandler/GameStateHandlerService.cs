using System.Collections.Generic;
using Code.Infrastructure.States.GameStateHandler.Handlers;

namespace Code.Infrastructure.States.GameStateHandler
{
    public class GameStateHandlerService : IGameStateHandlerService
    {
        private readonly List<IEnterBootstrapStateHandler> _enterBoostrtapStateHandlers;
        private readonly List<ILoadProgressStateHandler> _enterLoadProgressStateHandlers;
        private readonly List<IEnterMainMenuStateHandler> _mainMenuStateHandlers;
        private readonly List<IEnterGameLoopStateHandler> _enterGameLoopStateHandlers;
        private readonly List<IExitGameLoopStateHandler> _exitGameLoopStateHandlers;

        public GameStateHandlerService
        (
            List<IEnterBootstrapStateHandler> enterBoostrtapStateHandlers,
            List<ILoadProgressStateHandler> enterLoadProgressStateHandlers,
            List<IEnterMainMenuStateHandler> mainMenuStateHandlers,
            List<IEnterGameLoopStateHandler> enterGameLoopStateHandlers,
            List<IExitGameLoopStateHandler> exitGameLoopStateHandlers
        )
        {
            _enterBoostrtapStateHandlers = enterBoostrtapStateHandlers;
            _enterLoadProgressStateHandlers = enterLoadProgressStateHandlers;
            _enterGameLoopStateHandlers = enterGameLoopStateHandlers;
            _exitGameLoopStateHandlers = exitGameLoopStateHandlers;
            _mainMenuStateHandlers = mainMenuStateHandlers;
            
            _enterBoostrtapStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _enterLoadProgressStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _enterGameLoopStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _mainMenuStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
            _exitGameLoopStateHandlers.Sort((x, y) => x.OrderType.CompareTo(y.OrderType));
        }

        public void RegisterHandler(IOrderableHandler handler)
        {
            switch (handler)
            {
                case IEnterBootstrapStateHandler stateHandler:
                    _enterBoostrtapStateHandlers.Add(stateHandler);
                    break;
                case IEnterGameLoopStateHandler stateHandler:
                    _enterGameLoopStateHandlers.Add(stateHandler);
                    break;
                case IExitGameLoopStateHandler stateHandler:
                    _exitGameLoopStateHandlers.Add(stateHandler);
                    break;
                case IEnterMainMenuStateHandler stateHandler:
                    _mainMenuStateHandlers.Add(stateHandler);
                    break;
            }
        }

        public void OnEnterBootstrapState()
        {
            foreach (var handler in _enterBoostrtapStateHandlers) 
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