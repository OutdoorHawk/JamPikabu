using Code.Common.Extensions;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Input.Behaviours
{
    public class MobileInputBehaviour : MonoBehaviour, IEnterGameLoopStateHandler, IExitGameLoopStateHandler
    {
        private IGameStateHandlerService _gameStateHandler;

        public OrderType OrderType => OrderType.First;

        [Inject]
        private void Construct(IGameStateHandlerService gameStateHandler)
        {
            _gameStateHandler = gameStateHandler;
        }

        private void Awake()
        {
#if !UNITY_ANDROID
            Destroy(gameObject);
#else
            _gameStateHandler.RegisterHandler(this);
            gameObject.DisableElement();
#endif
        }

        public void OnEnterGameLoop()
        {
            gameObject.EnableElement();
        }

        public void OnExitGameLoop()
        {
            gameObject.DisableElement();
        }
    }
}