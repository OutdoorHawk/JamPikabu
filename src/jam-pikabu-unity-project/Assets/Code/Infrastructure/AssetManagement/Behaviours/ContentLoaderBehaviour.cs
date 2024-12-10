using Code.Common.Extensions;
using Code.Infrastructure.AssetManagement.AssetManagement;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.AssetManagement.Behaviours
{
    public class ContentLoaderBehaviour : MonoBehaviour
    {
        [SerializeField] private RectTransform _fillBarRect;

        private float _maxSizeX;
        private float _minSizeX;

        private IAssetDownloadReporter _downloadReporter;

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
            _downloadReporter.ProgressUpdated -= UpdateProgress;
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
            gameObject.DisableElement();
        }

        private void UpdateProgress()
        {
            float x = Mathf.Lerp(_minSizeX, _maxSizeX, _downloadReporter.Progress);
            _fillBarRect.sizeDelta = _fillBarRect.sizeDelta.SetX(x);
        }
    }
}