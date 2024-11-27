using System.Threading;
using Code.Infrastructure.DI.Installers;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Initialize
{
    [Injectable(typeof(IInitializationService))]
    public class InitializationService : IInitializationService
    {
        public bool InitializationComplete { get; private set; }

        private CancellationTokenSource _tokenSource;

        public void SetLevelPrepared()
        {
            InitializationComplete = true;
        }

        public void SetLevelNotPrepared()
        {
            InitializationComplete = false;
        }

        public async UniTask<bool> InitializationCompleteAsync(CancellationToken token)
        {
            await UniTask.WaitUntil(() => InitializationComplete, PlayerLoopTiming.Update, token);
            return !token.IsCancellationRequested;
        }
    }
}