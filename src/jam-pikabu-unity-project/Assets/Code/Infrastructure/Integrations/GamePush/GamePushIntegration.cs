using Code.Common.Logger.Service;
using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;
using GamePush;

namespace Code.Infrastructure.Integrations.GamePush
{
    public class GamePushIntegration : IIntegration
    {
        private readonly ILoggerService _logger;
        public OrderType InitOrder => OrderType.First;

        public GamePushIntegration(ILoggerService logger)
        {
            _logger = logger;
        }

        public async UniTask Initialize()
        {
            await GP_Init.Ready;
            _logger.Log($"Game push player id: {GP_Player.GetID()}");
        }
    }
}