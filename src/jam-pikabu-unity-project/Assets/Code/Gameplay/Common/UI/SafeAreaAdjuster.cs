using System;
using Code.Gameplay.Windows.Factory;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Common.UI
{
    public class SafeAreaHorizontalAdjuster : MonoBehaviour
    {
        [SerializeField] private RectTransform _targetRectTransform;
        
        private IUIFactory _uiFactory;
        private Vector2 _screenSafeAreaCached;

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

            if (_screenSafeAreaCached.Equals(Screen.safeArea.size))
                return;

            _screenSafeAreaCached = Screen.safeArea.size;
            _targetRectTransform.sizeDelta = _screenSafeAreaCached / _uiFactory.Canvas.scaleFactor;
        }
    }
}