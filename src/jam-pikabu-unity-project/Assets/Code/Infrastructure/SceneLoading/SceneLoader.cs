using System;
using System.Collections;
using Code.Common.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.SceneLoading
{
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        [SerializeField] private CanvasGroup _faderImage;
        [SerializeField] private Animator _spinerAnimator;
        [SerializeField] private float _fadeTime = 0.25f;

        private Coroutine _loadingRoutine;
        private Tween _imageTween;
        private Func<UniTask> _loadTask;

        public void LoadScene(SceneTypeId sceneID, Func<UniTask> loadOperation = null, Action onLoaded = null)
        {
            ResetLoadingRoutine();
            LoadScene(sceneID.ToString(), loadOperation, onLoaded);
        }

        public void LoadScene(string sceneName, Func<UniTask> loadOperation = null, Action onLoaded = null)
        {
            ResetLoadingRoutine();

            _loadTask = loadOperation;
            _spinerAnimator.SetBehaviorEnabled();
            _faderImage.blocksRaycasts = true;

            _imageTween?.Kill();
            _imageTween = _faderImage
                    .DOFade(1, _fadeTime)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .OnComplete(() => StartLoadingOperation(sceneName, onLoaded))
                ;
        }

        private void ResetLoadingRoutine()
        {
            if (_loadingRoutine != null)
                StopCoroutine(_loadingRoutine);

            _loadingRoutine = null;
        }

        private void StartLoadingOperation(string sceneName, Action onLoaded)
        {
            _loadingRoutine = StartCoroutine(LoadingScreenStartRoutine(sceneName, onLoaded));
        }

        private IEnumerator LoadingScreenStartRoutine(string sceneName, Action onLoaded)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            if (_loadTask != null)
            {
                UniTask task = _loadTask.Invoke();
                yield return task.ToCoroutine();
                _loadTask = null;
            }

            while (operation != null && !operation.isDone)
                yield return 0;

            _loadingRoutine = null;
            onLoaded?.Invoke();

            yield return new WaitForSeconds(0.5f);

            _faderImage.blocksRaycasts = false;

            _imageTween?.Kill();
            _imageTween = _faderImage
                    .DOFade(0, _fadeTime)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .OnComplete(_spinerAnimator.SetBehaviorDisabledSafe)
                ;
        }
    }
}