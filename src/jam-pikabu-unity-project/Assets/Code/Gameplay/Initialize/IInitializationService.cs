using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Initialize
{
    public interface IInitializationService
    {
        bool InitializationComplete { get; }
        void SetLevelPrepared();
        void SetLevelNotPrepared();
        UniTask<bool> InitializationCompleteAsync(CancellationToken token);
    }
}