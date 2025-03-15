using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.ABTesting
{
    public interface IABTestService
    {
        OrderType InitOrder { get; }
        UniTask Initialize();
        ExperimentValueTypeId GetExperimentValue(ExperimentTagTypeId tag);
    }
}