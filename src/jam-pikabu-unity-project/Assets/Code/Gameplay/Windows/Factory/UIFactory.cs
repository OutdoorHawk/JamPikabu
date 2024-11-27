using System;
using Code.Gameplay.StaticData;
using Code.Infrastructure.DI.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Gameplay.Windows.Factory
{
    public class UIFactory : IUIFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IZenjectFactory _zenjectFactory;

        private Transform _uiRoot;

        [Inject]
        public UIFactory(IStaticDataService staticDataService, IZenjectFactory zenjectFactory)
        {
            _zenjectFactory = zenjectFactory;
            _staticDataService = staticDataService;
        }

        public Transform UIRoot => _uiRoot;

        public void CreateUiRoot()
        {
            if (UIRoot != null)
                Object.Destroy(UIRoot.gameObject);

            throw new NotImplementedException();
            /*GameObject uiRoot = _zenjectFactory.Instantiate(_staticDataService.Data.WindowsStaticData.UIRoot.gameObject);
            uiRoot.transform.SetParent(null);
            _uiRoot = uiRoot.transform;*/
        }

        public async UniTask<T> CreateWindow<T>(WindowTypeId type) where T : BaseWindow
        {
            throw new NotImplementedException();

            /*WindowConfig config = _staticDataService.GetWindow(type);
            GameObject window = await _zenjectFactory.Instantiate(config.WindowName, UIRoot);
            T typedWindow = window.GetComponent<T>();
            typedWindow.SetWindowType(type);
            return typedWindow;*/
        }
    }
}