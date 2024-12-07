using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
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
        private ISoundService _soundService;
        private IGameOverService _gameOverService;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine, ISoundService soundService, IGameOverService gameOverService)
        {
            _gameOverService = gameOverService;
            _soundService = soundService;
            _gameStateMachine = gameStateMachine;
        }

        protected override void Initialize()
        {
            base.Initialize();
            PlaySound();
            RestartAfterDelay().Forget();
        }

        private async UniTask RestartAfterDelay()
        {
            await DelaySeconds(3, destroyCancellationToken);
            var loadLevelPayloadParameters = new LoadLevelPayloadParameters(SceneTypeId.Level_1.ToString());
            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(loadLevelPayloadParameters);
        }

        private void PlaySound()
        {
            if (_gameOverService.IsGameWin)
            {
                _soundService.PlayOneShotSound(SoundTypeId.Level_Win);
            }
            else
            {
                _soundService.PlayOneShotSound(SoundTypeId.Level_Lost);
            }
        }
    }
}