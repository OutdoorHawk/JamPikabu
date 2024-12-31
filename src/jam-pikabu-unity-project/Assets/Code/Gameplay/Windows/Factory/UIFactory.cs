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
        private Canvas _canvas;

        private const string PATH = "Windows/";

        [Inject]
        public UIFactory(IStaticDataService staticDataService, IInstantiator instantiator, IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            _instantiator = instantiator;
            _staticDataService = staticDataService;
        }

        public Transform UIRoot => _uiRoot;
        public Canvas Canvas => _canvas;

        public void InitializeCamera()
        {
            _canvas.worldCamera = Camera.main;
        }

        public void CreateUiRoot()
        {
            if (UIRoot != null)
                Object.Destroy(UIRoot.gameObject);

            WindowsStaticData windows = _staticDataService.Get<WindowsStaticData>();
            GameObject uiRoot = _instantiator.InstantiatePrefab(windows.UIRoot.gameObject);
            uiRoot.transform.SetParent(null);
            _uiRoot = uiRoot.transform;
            _canvas = _uiRoot.GetComponent<Canvas>();
            _graphicRaycaster = _canvas.GetComponent<GraphicRaycaster>();
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
        
        public Vector3 GetWorldPositionForUI(Vector3 worldPos)
        {
            return GetWorldPositionForUIInternal(worldPos);
        }
        
        public Vector3 GetWorldPositionFromScreenPosition(Vector3 screenPos)
        {
            return GetWorldPositionFromScreenPositionInternal(screenPos);
        }

        private BaseWindow GetWindowPrefab(WindowTypeId type)
        {
            WindowsStaticData windows = _staticDataService.Get<WindowsStaticData>();
            WindowConfig config = windows.GetWindow(type);
            BaseWindow windowPrefab = config.WindowPrefab;
            windowPrefab ??= _assetProvider.LoadAssetFromResources<BaseWindow>(PATH + config.WindowName);
            return windowPrefab;
        }

        private Vector3 GetWorldPositionForUIInternal(Vector3 worldPos)
        {
            Camera camera = _canvas.worldCamera;
            
            // Преобразуем мировые координаты в экранные
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPos);

            // Преобразуем экранные координаты обратно в мировые координаты с учетом Canvas
            Canvas canvas = UIRoot.GetComponent<Canvas>();
            RectTransform canvasRect = canvas.transform as RectTransform;

            // Учитываем положение и размер Canvas
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPosition, camera, out Vector3 uiWorldPosition))
            {
                return uiWorldPosition;
            }

            Debug.LogWarning("Failed to convert world position to UI world position.");
            return Vector3.zero;
        }

        private Vector3 GetWorldPositionFromScreenPositionInternal(Vector3 screenPos)
        {
            Camera camera = _canvas.worldCamera;
            Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(camera, screenPos);
            screenPosition.z = Mathf.Abs(camera.transform.position.z);
            Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;
            return worldPosition;
        }
    }
}