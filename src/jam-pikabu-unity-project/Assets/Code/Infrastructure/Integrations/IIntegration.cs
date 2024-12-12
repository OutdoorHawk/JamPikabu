using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Integrations
{
    public interface IIntegration
    {
        OrderType InitOrder { get; }
        UniTask Initialize();
    }
}