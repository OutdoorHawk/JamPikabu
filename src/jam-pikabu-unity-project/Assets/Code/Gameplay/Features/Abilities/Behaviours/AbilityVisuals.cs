using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Features.Abilities.Behaviours
{
    public class AbilityVisuals : MonoBehaviour
    {
        public Rigidbody2D Rigidbody;

        public float SwapDuration = 0.25f;

        public float ChangeSizeDuration = 0.5f;
        public float BigSize = 1.25f;
        public float SmallSize = 0.75f;

        private Tween _tween;

        private void OnDestroy()
        {
            ResetTween();
        }

        public void PlaySwap(Vector3 newPosition)
        {
            PlaySwapAsync(newPosition).Forget();
        }

        public void ChangeSize()
        {
            bool isBigSize = Mathf.Approximately(transform.localScale.x, BigSize);
            bool isSmallSize = Mathf.Approximately(transform.localScale.x, SmallSize);
            ResetTween();

            if (isBigSize || isSmallSize)
            {
                _tween = transform
                    .DOScale(1, ChangeSizeDuration)
                    .SetEase(Ease.OutBounce)
                    .SetLink(gameObject);

                return;
            }

            if (Random.Range(0, 2) == 0)
            {
                _tween = transform
                    .DOScale(SmallSize, ChangeSizeDuration)
                    .SetEase(Ease.OutBounce)
                    .SetLink(gameObject);
            }
            else
            {
                _tween = transform
                    .DOScale(BigSize, ChangeSizeDuration)
                    .SetEase(Ease.OutBounce)
                    .SetLink(gameObject);
            }
        }

        private async UniTaskVoid PlaySwapAsync(Vector3 newPosition)
        {
            ResetTween();
            
            CancellationToken cts = gameObject.GetCancellationTokenOnDestroy();

            _tween = transform
                .DOScale(1.2f, SwapDuration / 3)
                .SetLink(gameObject,  LinkBehaviour.KillOnDestroy);

            await _tween.AsyncWaitForCompletion();

            if (cts.IsCancellationRequested)
                return;

            _tween = transform
                .DOScale(0, SwapDuration)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            await _tween.AsyncWaitForCompletion();
            
            if (cts.IsCancellationRequested)
                return;

            if (Rigidbody == null)
                return;

            Rigidbody.position = newPosition;

            _tween = transform
                .DOScale(1, SwapDuration)
                .SetLink(gameObject,  LinkBehaviour.KillOnDestroy)
                .SetEase(Ease.OutBounce);
        }

        private void ResetTween()
        {
            _tween?.Kill();
        }
    }
}