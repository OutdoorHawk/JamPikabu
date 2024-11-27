using System;
using System.Collections;
using System.Threading;
using Code.Common.Extensions.Animations;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Common.Extensions
{
    public static class AsyncGameplayExtensions
    {
        private const float MIN_VALUE = 0.0001F;

        public static double Lerp(double start, double end, double t)
        {
            return start + (end - start) * Math.Clamp(t, 0d, 1d);
        }
        
        public static void SetTriggerSafe(this Animator animator, AnimationParameter parameter)
        {
            if (animator != null) 
                animator.SetTrigger(parameter.AsHash());
        }

        public static async UniTask BlockButtonClick(this Button button)
        {
            button.SetBehaviorDisabled();
            const float blockTime = 0.5f;
            await DelaySeconds(blockTime, button.destroyCancellationToken);
            button.SetBehaviorEnabled();
        }

        public static void RestartCoroutine(this MonoBehaviour behaviour, ref Coroutine coroutine, IEnumerator routineMethod)
        {
            if (behaviour == null)
                return;

            if (coroutine != null)
                behaviour.StopCoroutine(coroutine);

            coroutine = behaviour.StartCoroutine(routineMethod);
        }

        public static void StopCoroutineSafe(this MonoBehaviour behaviour, ref Coroutine coroutine)
        {
            if (behaviour == null)
                return;

            if (coroutine != null)
                behaviour.StopCoroutine(coroutine);

            coroutine = null;
        }

        public static async UniTask WaitForAnimationCompleteAsync(this Animator animator, int hash, CancellationToken token)
        {
            animator.ResetTrigger(hash);
            animator.SetTrigger(hash);
            await UniTask.NextFrame(token);
            float animTime = GetCurrentAnimationLength(animator);
            await DelaySeconds(animTime, token);
        }

        public static async UniTask WaitForAnimationCompleteAsync(Animator animator, CancellationToken token)
        {
            await UniTask.NextFrame(token);
            float animTime = GetCurrentAnimationLength(animator);
            await DelaySeconds(animTime, token);
        }

        public static async UniTask DelaySeconds(float delay, CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
        }

        public static async UniTaskVoid ResetAnimationAsync(this Animator animator, int hash, CancellationToken token)
        {
            animator.SetBehaviorEnabled();
            animator.ResetTrigger(hash);
            animator.SetTrigger(hash);
            await UniTask.NextFrame(token);
            animator.SetBehaviorDisabled();
        }

        public static async UniTaskVoid SetTriggerOnNextFrameAsync(this Animator animator, int hash, CancellationToken token)
        {
            await UniTask.NextFrame(token);
            animator.SetTrigger(hash);
        }

        public static async UniTaskVoid SetTriggerWithDelayAsync(this Animator animator, int hash, float delay, CancellationToken token)
        {
            await DelaySeconds(delay, token);
            animator.SetTrigger(hash);
        }

        public static async UniTask ResetAnimationToFirstFrameAsync(this Animator animator, CancellationToken token)
        {
            var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);

            if (currentClipInfo.Length == 0)
                return;

            AnimationClip clip = currentClipInfo[0].clip;

            animator.SetBehaviorEnabled();
            animator.Play(clip.name, 0, 0);

            await UniTask.NextFrame(token);

            animator.SetBehaviorDisabledSafe();
            ;
        }

        public static async UniTaskVoid SetLastAnimationFrameAsync(this Animator animator, int hash, CancellationToken token)
        {
            animator.SetBehaviorEnabled();
            animator.ResetTrigger(hash);
            animator.SetTrigger(hash);

            await UniTask.NextFrame(token);

            var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);

            if (currentClipInfo.Length == 0)
                return;

            AnimationClip clip = currentClipInfo[0].clip;
            animator.Play(clip.name, 0, clip.length);
        }

        public static float GetCurrentAnimationLength(this Animator animator)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            return animState.length;
        }

        private static float ClampDuration(float duration)
        {
            return Mathf.Clamp(duration, MIN_VALUE, Mathf.Infinity);
        }

        public static async UniTask ToColorAsync(this Image image, Color color, float duration, CancellationToken token,
            AnimationCurve curve = null)
        {
            float startTime = Time.time;
            Color startValue = image.color;
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                image.color = Color.LerpUnclamped(startValue, color, t);
                await UniTask.Yield(token);
            }

            image.color = color;
        }

        public static async UniTask ToFadeAsync(this Image image, float alpha, float duration, CancellationToken token,
            AnimationCurve curve = null)
        {
            float startTime = Time.time;
            Color startValue = image.color;
            Color endValue = new Color(startValue.r, startValue.g, startValue.b, alpha);
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                image.color = Color.LerpUnclamped(startValue, endValue, t);
                await UniTask.Yield(token);
            }

            image.color = endValue;
        }
        
        public static async UniTask ToAnchorPos(this RectTransform rect, Vector2 endValue, float duration,
            CancellationToken token, AnimationCurve curve = null)
        {
            float startTime = Time.time;
            Vector2 startValue = rect.anchoredPosition;
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                rect.anchoredPosition = Vector2.LerpUnclamped(startValue, endValue, t);
                await UniTask.Yield(token);
            }

            rect.anchoredPosition = endValue;
        }

        public static async UniTask ToPos(this Transform transform, Vector2 endValue, float duration,
            CancellationToken token, AnimationCurve curve = null)
        {
            float startTime = Time.time;
            Vector2 startValue = transform.position;
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                transform.position = Vector2.LerpUnclamped(startValue, endValue, t);
                await UniTask.Yield(token);
            }

            transform.position = endValue;
        }

        public static async UniTask ToSizeDelta(this RectTransform rect, Vector2 endValue, float duration,
            CancellationToken token, AnimationCurve curve = null)
        {
            float startTime = Time.time;
            Vector2 startValue = rect.sizeDelta;
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                rect.sizeDelta = Vector2.LerpUnclamped(startValue, endValue, t);
                await UniTask.Yield(token);
            }

            rect.sizeDelta = endValue;
        }

        public static async UniTask ToScale(this Transform transform, Vector3 endValue, float duration, CancellationToken token,
            AnimationCurve curve = null)
        {
            float startTime = Time.time;
            Vector2 startValue = transform.localScale;
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                transform.localScale = Vector3.LerpUnclamped(startValue, endValue, t);
                await UniTask.Yield(token);
            }

            transform.localScale = endValue;
        }

        public static async UniTask ToMinHeight(this LayoutElement layoutElement, float endValue, float duration,
            CancellationToken token, AnimationCurve curve = null)
        {
            float startTime = Time.time;
            float startValue = layoutElement.minHeight;
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                layoutElement.minHeight = Mathf.LerpUnclamped(startValue, endValue, t);
                await UniTask.Yield(token);
            }

            layoutElement.minHeight = endValue;
        }

        public static async UniTask ToTextValue(this TMP_Text text, double startValue, double endValue, float duration,
            CancellationToken token, AnimationCurve curve = null)
        {
            float startTime = Time.time;
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                text.text = Lerp(startValue, endValue, t).ToString("#");
                await UniTask.Yield(token);
            }

            text.text = endValue.ToString("#");
        }

        public static async UniTask ToTextValue(this TMP_Text text, int startValue, int endValue, float duration,
            CancellationToken token, AnimationCurve curve = null, string format = "#")
        {
            float startTime = Time.time;
            float clampedDuration = ClampDuration(duration);

            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;

                text.text = Lerp(startValue, endValue, t).ToString(format);
                await UniTask.Yield(token);
            }

            text.text = endValue.ToString(format);
        }

        public static async UniTask ToScrollPosition(this ScrollRect scrollRect, float endValue, float duration,
            CancellationToken token, AnimationCurve curve = null)
        {
            float startTime = Time.time;
            float startValue = scrollRect.normalizedPosition.y;
            float clampedDuration = ClampDuration(duration);
            Vector2 tempPosition = scrollRect.normalizedPosition;
            while (Time.time - startTime < duration)
            {
                float elapsedTime = Time.time - startTime;
                float progress = elapsedTime / clampedDuration;
                float t = curve?.Evaluate(progress) ?? progress;
                tempPosition.y = Mathf.LerpUnclamped(startValue, endValue, t);
                scrollRect.normalizedPosition = tempPosition;
                await UniTask.Yield(token);
            }

            tempPosition.y = endValue;
            scrollRect.normalizedPosition = tempPosition;
        }
    }
}