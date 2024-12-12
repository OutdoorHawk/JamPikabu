using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;
using GamePush;

namespace Code.Infrastructure.Integrations.GamePush
{
    public class GamePushIntegration : IIntegration
    {
        public OrderType InitOrder => OrderType.First;

        public async UniTask Initialize()
        {
            await GP_Init.Ready;
        }
    }
}