using System.Collections.Generic;
using System.Threading;
using Code.Common.Extensions;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Localization;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static System.Threading.CancellationTokenSource;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Tutorial.Window
{
    public class TutorialWindow : BaseWindow
    {
        [SerializeField] private TutorialMessageBox[] _messageBoxes;
        [SerializeField] private GameObject _blackBackground;
        [SerializeField] private CanvasGroup _tapToContinueText;
        [SerializeField] private Button _tapToContinueButton;
        [SerializeField] private RectTransform _arrow;

        private ILocalizationService _localizationService;
        private IWindowService _windowService;
        private Tween _textTween;

        private Transform _uiRoot;
        private TutorialMessageBox _currentMessage;
        private (RectTransform rect, Vector2 offset) _arrowTarget;

        private readonly List<(Canvas, GraphicRaycaster)> _highlightedObjects = new();
        private readonly Dictionary<TutorialMessageAnchorType, TutorialMessageBox> _messageBoxesDict = new();

        private const string LOCALE_BASE = "T_";
        private const int HIGHLIGHT_SORT_ORDER = 2;

        [Inject]
        private void Construct
        (
            IUIFactory uiFactory,
            ILocalizationService localizationService,
            IWindowService windowService
        )
        {
            _windowService = windowService;
            _localizationService = localizationService;
            _uiRoot = uiFactory.UIRoot;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            HideAll();
            foreach (var box in _messageBoxes)
                _messageBoxesDict[box.AnchorType] = box;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            HideAll();
        }

        private void Update()
        {
            if (_arrowTarget.rect == null || _arrow == null || _uiRoot == null)
                return;

            RectTransform targetRect = _arrowTarget.rect;
            if (targetRect == null)
                return;

            // Получаем мировые координаты цели
            Vector3 worldPosition = targetRect.position + (Vector3)(_arrowTarget.offset * _uiRoot.localScale);

            // Преобразуем мировые координаты в экранные
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);

            // Преобразуем экранные координаты обратно в локальные для канваса
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_uiRoot, screenPosition, Camera.main, out Vector2 localPosition);

            // Устанавливаем позицию стрелки
            _arrow.localPosition = localPosition;
        }

        public TutorialWindow ShowMessage(int locale, string arg1 = null, TutorialMessageAnchorType anchorType = TutorialMessageAnchorType.VeryTop)
        {
            string localizedText = GetLocalizedText(locale, arg1);
            if (_currentMessage != null && _currentMessage.AnchorType != anchorType)
                _currentMessage.PlayHide();
            _currentMessage = _messageBoxesDict[anchorType];
            _currentMessage.Show(localizedText);
            return this;
        }

        public TutorialWindow ShowDarkBackground()
        {
            _blackBackground.SetActive(true);
            return this;
        }

        public TutorialWindow HideDarkBackground()
        {
            _blackBackground.SetActive(false);
            return this;
        }

        public TutorialWindow HideMessages()
        {
            foreach (var box in _messageBoxes)
                box.PlayHide();

            return this;
        }

        public TutorialWindow HideWindow()
        {
            CloseWindowInternal();
            return this;
        }

        public TutorialWindow ShowArrow(Transform rect, Vector2? offset = null, ArrowRotation rotation = ArrowRotation.Top)
        {
            return ShowArrow(rect as RectTransform, offset, rotation);
        }

        public TutorialWindow ShowArrow
        (
            Transform rect,
            float xOffset = 0,
            float yOffset = 0,
            ArrowRotation rotation = ArrowRotation.Top
        )
        {
            return ShowArrow(rect as RectTransform, new Vector2(xOffset, yOffset), rotation);
        }

        public TutorialWindow ShowArrow
        (
            RectTransform rect,
            Vector2? offset = null,
            ArrowRotation rotation = ArrowRotation.Top
        )
        {
            if (offset != null)
                _arrowTarget.offset = offset.Value;

            _arrowTarget.rect = rect;
            _arrow.rotation = Quaternion.Euler(GetRotationVector(rotation));
            _arrow.EnableElement();
            return this;
        }

        public TutorialWindow HideArrow()
        {
            _arrow.DisableElement();
            _arrowTarget.rect = null;
            _arrowTarget.offset = Vector2.zero;
            return this;
        }

        public TutorialWindow HighlightObject(MonoBehaviour rect)
        {
            HighlightObject(rect.gameObject);
            return this;
        }

        public TutorialWindow HighlightObject(GameObject rect)
        {
            var canvas = rect.AddComponent<Canvas>();
            var caster = rect.AddComponent<GraphicRaycaster>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Tutorial";
            canvas.sortingOrder = HIGHLIGHT_SORT_ORDER;
            _highlightedObjects.Add((canvas, caster));
            return this;
        }

        public TutorialWindow ClearHighlights()
        {
            foreach ((Canvas, GraphicRaycaster) tuple in _highlightedObjects)
            {
                if (tuple.Item2 != null)
                    Destroy(tuple.Item2);
                if (tuple.Item1 != null)
                    Destroy(tuple.Item1);
            }

            _highlightedObjects.Clear();
            return this;
        }

        public async UniTask AwaitForTapAnywhere(CancellationToken token, float delay = 0)
        {
            _tapToContinueText.EnableElement();
            _tapToContinueButton.EnableElement();
            _tapToContinueText.alpha = 0;
            
            await DelaySeconds(delay, token);
            
            _textTween?.Kill();
            _textTween = _tapToContinueText
                    .DOFade(1, 0.25f)
                    .SetLink(_tapToContinueText.gameObject)
                ;

            await _tapToContinueButton.OnClickAsync(CreateLinkedTokenSource(token, destroyCancellationToken).Token);
            _tapToContinueButton.DisableElement();
            _tapToContinueText.DisableElement();
        }

        private string GetLocalizedText(int locale, string arg1 = null)
        {
            string result = arg1 == null
                ? _localizationService[$"TUTORIAL/{LOCALE_BASE}{locale.ToString()}"]
                : _localizationService[$"TUTORIAL/{LOCALE_BASE}{locale.ToString()}", arg1];

            return result;
        }

        private void HideAll()
        {
            HideMessages();

            ClearHighlights();
            _blackBackground.DisableElement();
            _arrow.DisableElement();
            _tapToContinueText.DisableElement();

        }

        private Vector3 GetRotationVector(ArrowRotation handPosition)
        {
            switch (handPosition)
            {
                case ArrowRotation.Top: return Vector3.forward * 0;
                case ArrowRotation.Left: return Vector3.forward * 90;
                case ArrowRotation.Bottom: return Vector3.forward * 180;
                case ArrowRotation.Right: return Vector3.forward * 270;
                default: return Vector3.zero;
            }
        }
    }
}