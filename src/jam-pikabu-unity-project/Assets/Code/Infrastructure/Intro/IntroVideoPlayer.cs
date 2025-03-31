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

        private const string VideoPath = "https://s3.gamepush.com/games/18323/v120/StreamingAssets/intro_video.mp4";
        
        public async UniTask WaitForVideoCompleteAsync()
        {
            VideoPlayer.url = VideoPath;
            PlayerImage.color = Color.white;
            VideoPlayer.Prepare();
            UniTask preparedTask = PreparedTask();
            UniTask timeOutTask = TimeOutTask();
            await UniTask.WhenAny(preparedTask, timeOutTask);
            
            if (VideoPlayer.isPrepared)
            {
                VideoPlayer.Play();
                await DelaySeconds((float)VideoPlayer.length, destroyCancellationToken);
            }
        }

        private async UniTask PreparedTask()
        {
            await UniTask.WaitUntil(() => VideoPlayer.isPrepared, cancellationToken: destroyCancellationToken);
        }

        private async UniTask TimeOutTask()
        {
            const int prepareTimeout = 3;
            await DelaySeconds(prepareTimeout, destroyCancellationToken);
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