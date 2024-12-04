using Code.Gameplay.Windows;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.GameOver.Windows
{
    public class GameOverWindow : BaseWindow
    {
        private IGameStateMachine _gameStateMachine;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        protected override void Initialize()
        {
            base.Initialize();
            RestartAfterDelay().Forget();
        }

        private async UniTask RestartAfterDelay()
        {
            await DelaySeconds(3, destroyCancellationToken);
            var loadLevelPayloadParameters = new LoadLevelPayloadParameters(SceneTypeId.Level_1.ToString());
            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(loadLevelPayloadParameters);
        }
    }
}