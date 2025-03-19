using Code.Common.Extensions;
using Code.Infrastructure.AssetManagement.AssetDownload;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Infrastructure.AssetManagement.Behaviours
{
    public class ContentLoaderBehaviour : MonoBehaviour
    {
        [SerializeField] private RectTransform _fillBarRect;
        [SerializeField] private float _fillDuration = 0.15f;

        private float _maxSizeX;
        private float _minSizeX;

        private IAssetDownloadReporter _downloadReporter;
        private Tweener _tweener;

        [Inject]
        private void Construct(IAssetDownloadReporter downloadReporter)
        {
            _downloadReporter = downloadReporter;
        }

        private void Awake()
        {
            _maxSizeX = _fillBarRect.sizeDelta.x;
            _minSizeX = 0;
        }

        private void Start()
        {
            _downloadReporter.ProgressUpdated += UpdateProgress;
        }

        private void OnDestroy()
        {
            _downloadReporter.ProgressUpdated -= UpdateProgress;
        }

        public void Init()
        {
            gameObject.EnableElement();
            _fillBarRect.sizeDelta = _fillBarRect.sizeDelta.SetX(0);
        }

        public void Hide()
        {
            if (_tweener != null)
            {
                HideAsync().Forget();
                return;
            }

            gameObject.DisableElement();
            Destroy(gameObject);
        }

        private async UniTaskVoid HideAsync()
        {
            await DelaySeconds(_fillDuration, destroyCancellationToken);
            gameObject.DisableElement();
            Destroy(gameObject);
        }

        private void UpdateProgress()
        {
            float progress = _downloadReporter.Progress;
            float x = Mathf.Lerp(_minSizeX, _maxSizeX, progress);
            
            _tweener?.Kill();
            _tweener = _fillBarRect
                    .DOSizeDelta(_fillBarRect.sizeDelta.SetX(x), _fillDuration)
                    .SetLink(gameObject)
                    .OnComplete(() =>
                    {
                        _tweener?.Kill();
                        _tweener = null;
                    })
                ;
        }
    }
}