using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.Intro
{
    public class IntroAnimator : MonoBehaviour
    {
        public IntroVideoPlayer VideoPlayer;

        private UniTaskCompletionSource _animationFinishedSource;

        private void Awake()
        {
            InitSource(destroyCancellationToken);
        }

        private void Start()
        {
            PlayIntroAnimation().Forget();
        }

        public async UniTask WaitForAnimationCompleteAsync()
        {
#if !UNITY_EDITOR
            await _animationFinishedSource.Task;
            await VideoPlayer.HidePlayer();
#endif
            Destroy(gameObject);
        }

        private void InitSource(CancellationToken cancellationToken)
        {
            _animationFinishedSource = new UniTaskCompletionSource();
            destroyCancellationToken.Register(() => _animationFinishedSource.TrySetCanceled(cancellationToken));
        }

        private async UniTaskVoid PlayIntroAnimation()
        {
            await VideoPlayer.WaitForVideoCompleteAsync();
            _animationFinishedSource.TrySetResult();
        }
    }
}