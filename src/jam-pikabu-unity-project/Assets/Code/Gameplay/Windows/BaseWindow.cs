using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Localization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Windows
{
    public abstract class BaseWindow : MonoBehaviour
    {
        public Button CloseButton;
        public Button[] CloseButtons;

        [SerializeField] private bool _useAnimation;
        [SerializeField] private bool _canCloseByBack;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _animationDuration = 0.15f;
        
        private Tweener _animationTweener;
        private bool _isClosing;

        public bool BlockClosing { get; set; }
        public bool CanCloseByBack => _canCloseByBack;
        public WindowTypeId WindowType { get; private set; }
        protected IWindowService WindowService { get; set; }
        protected ILocalizationService LocalizationService { get; private set; }


        [Inject]
        private void Construct(IWindowService windowService, ILocalizationService localizationService)
        {
            LocalizationService = localizationService;
            WindowService = windowService;
        }

        private void Awake()
        {
            _isClosing = false;
            _canvasGroup ??= GetComponent<CanvasGroup>();
            OnAwake();
        }

        private void Start()
        {
            Initialize();
            SubscribeUpdates();
            LoadStartupData();
            TryPlayStartAnimation();
        }

        private void OnDestroy()
        {
            Cleanup();
            _animationTweener?.Kill();
        }

        public void SetWindowType(WindowTypeId type)
        {
            WindowType = type;
        }

        public void SetCanCloseByBack(bool canClose)
        {
            _canCloseByBack = canClose;
        }

        public void SetCloseButtonInteractable(bool state)
        {
            CloseButton.interactable = state;
            
            foreach (var button in CloseButtons) 
                button.interactable = state;
        }

        public void Close()
        {
            if (BlockClosing)
                return;
            
            CloseWindowInternal();
        }

        protected virtual void Initialize()
        {
           
        }

        protected virtual void OnAwake()
        {
          
        }

        protected virtual void SubscribeUpdates()
        {
            if (CloseButton != null)
                CloseButton.onClick.AddListener(CloseWindowInternal);

            foreach (var button in CloseButtons) 
                button.onClick.AddListener(CloseWindowInternal);

            if (this is ILocalizationHandler handler) 
                LocalizationService.RegisterHandler(handler);
        }

        protected virtual void Unsubscribe()
        {
            if (CloseButton != null)
                CloseButton.onClick.RemoveListener(CloseWindowInternal);

            foreach (var button in CloseButtons) 
                button.onClick.RemoveListener(CloseWindowInternal);
            
            if (this is ILocalizationHandler handler) 
                LocalizationService.UnregisterHandler(handler);
        }

        protected virtual void LoadStartupData()
        {
        }

        protected virtual void Cleanup()
        {
            Unsubscribe();
        }

        protected virtual void CloseWindowInternal()
        {
            if (_isClosing)
                return;

            _isClosing = true;
            WindowService.RemoveWindowFromHistory(this);
            Cleanup();
            FinalizeCloseWindow();
        }

        private void TryPlayStartAnimation()
        {
            if (!_useAnimation)
                return;

            _canvasGroup.alpha = 0;
            _animationTweener?.Kill();
            _animationTweener = _canvasGroup
                .DOFade(1, _animationDuration)
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        private void FinalizeCloseWindow()
        {
            if (_useAnimation)
                PlayCloseAnimation();
            else
                Destroy(gameObject);
        }

        private void PlayCloseAnimation()
        {
            _animationTweener?.Kill();
            _animationTweener = _canvasGroup
                .DOFade(0, _animationDuration)
                .SetLink(gameObject)
                .SetUpdate(true)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}