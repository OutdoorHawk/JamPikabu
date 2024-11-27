using Code.Gameplay.Windows.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Windows.Behaviours
{
    [RequireComponent(typeof(Button))]
    public class OpenWindowButton : MonoBehaviour
    {
        [SerializeField] private WindowTypeId _windowType;

        private IWindowService _windowService;
        private Button _button;

        public WindowTypeId WindowType => _windowType;

        public Button Button => _button;

        [Inject]
        private void Construct(IWindowService windowService)
        {
            _windowService = windowService;
        }

        private void Awake()
        {
            _button ??= GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(OpenWindow);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenWindow);
        }

        private void OpenWindow()
        {
            _windowService.OpenWindow(WindowType);
        }
    }
}