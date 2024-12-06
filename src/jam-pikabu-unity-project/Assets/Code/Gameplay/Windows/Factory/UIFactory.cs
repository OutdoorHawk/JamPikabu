using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Configs;
using Code.Infrastructure.AssetManagement.AssetProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Gameplay.Windows.Factory
{
    public class UIFactory : IUIFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IInstantiator _instantiator;
        private readonly IAssetProvider _assetProvider;

        private Transform _uiRoot;
        private GraphicRaycaster _graphicRaycaster;

        private const string PATH = "Windows/";

        [Inject]
        public UIFactory(IStaticDataService staticDataService, IInstantiator instantiator, IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            _instantiator = instantiator;
            _staticDataService = staticDataService;
        }

        public Transform UIRoot => _uiRoot;

        public void InitializeCamera()
        {
            var canvas = _uiRoot.GetComponent<Canvas>();
            _graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            canvas.worldCamera = Camera.main;
        }

        public void CreateUiRoot()
        {
            if (UIRoot != null)
                Object.Destroy(UIRoot.gameObject);

            WindowsStaticData windows = _staticDataService.GetStaticData<WindowsStaticData>();
            GameObject uiRoot = _instantiator.InstantiatePrefab(windows.UIRoot.gameObject);
            uiRoot.transform.SetParent(null);
            _uiRoot = uiRoot.transform;
            
        }

        public void SetRaycastAvailable(bool available)
        {
            _graphicRaycaster.enabled = available;
        }

        public UniTask<T> CreateWindow<T>(WindowTypeId type) where T : BaseWindow
        {
            BaseWindow windowPrefab = GetWindowPrefab(type);
            BaseWindow window = _instantiator.InstantiatePrefabForComponent<BaseWindow>(windowPrefab, UIRoot);
            
            T typedWindow = window.GetComponent<T>();
            typedWindow.SetWindowType(type);
            return new UniTask<T>(typedWindow);
        }

        private BaseWindow GetWindowPrefab(WindowTypeId type)
        {
            WindowsStaticData windows = _staticDataService.GetStaticData<WindowsStaticData>();
            WindowConfig config = windows.GetWindow(type);
            BaseWindow windowPrefab = config.WindowPrefab;
            windowPrefab ??= _assetProvider.LoadAssetFromResources<BaseWindow>(PATH + config.WindowName);
            return windowPrefab;
        }
    }
}