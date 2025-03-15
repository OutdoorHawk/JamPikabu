using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Integrations
{
    public interface IIntegration
    {
        public OrderType InitOrder { get; }
        public UniTask Initialize();
    }
}