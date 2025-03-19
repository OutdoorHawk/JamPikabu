using Code.Common.Logger.Service;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Infrastructure.Intro
{
    public class IntroVideoPlayer : MonoBehaviour
    {
        public VideoPlayer VideoPlayer;
        public RawImage PlayerImage;

        private static readonly string VideoPath = Application.streamingAssetsPath + "/Intro.mp4";

        private ILoggerService _loggerService;

        [Inject]
        private void Construct
        (
            ILoggerService loggerService
        )
        {
            _loggerService = loggerService;
        }

        public async UniTask WaitForVideoCompleteAsync()
        {
            VideoPlayer.url = VideoPath;
            PlayerImage.color = Color.white;
            VideoPlayer.Prepare();
            await UniTask.WaitUntil(() => VideoPlayer.isPrepared, cancellationToken: destroyCancellationToken);
            VideoPlayer.Play();
            await DelaySeconds((float)VideoPlayer.length, destroyCancellationToken);
        }

        public async UniTask HidePlayer()
        {
            await PlayerImage
                    .DOFade(0, 0.25f)
                    .SetLink(gameObject)
                    .AsyncWaitForCompletion()
                    .AsUniTask()
                ;
        }
    }
}