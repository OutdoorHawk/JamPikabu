using Code.Gameplay.Windows.Factory;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Common.UI
{
    public class SafeAreaHorizontalAdjuster : MonoBehaviour
    {
        [SerializeField] private RectTransform _targetRectTransform;
        
        private IUIFactory _uiFactory;

        [Inject]
        private void Construct
        (
            IUIFactory uiFactory
        )
        {
            _uiFactory = uiFactory;
        }

        private void Start()
        {
            ApplySafeAreaAdjustment();
        }

        private void ApplySafeAreaAdjustment()
        {
            if (_targetRectTransform == null)
                return;

            _targetRectTransform.sizeDelta = Screen.safeArea.size / _uiFactory.Canvas.scaleFactor;
        }
    }
}