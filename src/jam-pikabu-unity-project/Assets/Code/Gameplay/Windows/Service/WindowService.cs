using System;
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Input.Service;
using Code.Gameplay.Windows.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Gameplay.Windows.Service
{
    public class WindowService : IWindowService
    {
        private readonly IUIFactory _uiFactory;
        private readonly Dictionary<WindowTypeId, BaseWindow> _windowsHistory = new();
        private readonly Dictionary<Type, BaseWindow> _windowsTypesHistory = new();

        public IReadOnlyDictionary<WindowTypeId, BaseWindow> Windows => _windowsHistory;

        [Inject]
        public WindowService(IUIFactory uiFactory, IInputService inputService)
        {
            _uiFactory = uiFactory;
            inputService.PlayerInput.Player.Escape.performed += CloseCurrentWindow;
        }

        public async UniTask<T> OpenWindow<T>(WindowTypeId type) where T : BaseWindow
        {
            if (IsWindowOpen(type))
                return GetWindow<T>(type);

            T window = await _uiFactory.CreateWindow<T>(type);
            AddWindowToHistory(type, window);
            return GetWindow<T>(type);
        }

        public void OpenWindow(WindowTypeId type)
        {
            if (IsWindowOpen(type))
                return;

            OpenWindowAsync(type).Forget();
        }

        public bool TryGetWindow<T>(WindowTypeId type, out T window) where T : class
        {
            if (_windowsHistory.TryGetValue(type, out BaseWindow openedWindow))
            {
                window = openedWindow as T;
                return true;
            }

            window = null;
            return false;
        }

        public bool TryGetWindow<T>(out T window) where T : class
        {
            if (_windowsTypesHistory.TryGetValue(typeof(T), out BaseWindow openedWindow))
            {
                window = openedWindow as T;
                return true;
            }

            window = null;
            return false;
        }

        public T[] GetWindows<T>() where T : class
        {
            return _windowsHistory.Values.OfType<T>().ToArray();
        }

        public void Close(WindowTypeId type)
        {
            if (!IsWindowOpen(type))
                return;

            BaseWindow window = _windowsHistory[type];
            window.Close();
        }

        public bool IsWindowOpen(WindowTypeId type)
        {
            return _windowsHistory.ContainsKey(type);
        }

        public void RemoveWindowFromHistory(BaseWindow window)
        {
            _windowsHistory.Remove(window.WindowType);
            _windowsTypesHistory.Remove(window.GetType());
        }

        public void ClearUIRoot()
        {
            foreach (BaseWindow window in _windowsHistory.Values)
                Object.Destroy(window.gameObject);

            _windowsHistory.Clear();
            _windowsTypesHistory.Clear();
        }

        public bool AnyWindowOpen()
        {
            if (_windowsHistory.Count == 0)
                return false;

            return true;
        }

        private async UniTaskVoid OpenWindowAsync(WindowTypeId type)
        {
            BaseWindow window = await _uiFactory.CreateWindow<BaseWindow>(type);
            AddWindowToHistory(type, window);
        }

        private void AddWindowToHistory(WindowTypeId type, BaseWindow window)
        {
            if (_windowsHistory.TryAdd(type, window) == false)
                Object.Destroy(window);
            else
                _windowsTypesHistory.Add(window.GetType() ,window);
        }

        private T GetWindow<T>(WindowTypeId type) where T : BaseWindow
        {
            return _windowsHistory[type] as T;
        }

        private void CloseCurrentWindow(InputAction.CallbackContext _)
        {
            CloseCurrentWindow();
        }

        private void CloseCurrentWindow()
        {
            if (_windowsHistory.Count == 0)
                return;

            BaseWindow currentWindow = _windowsHistory.Last().Value;

            if (currentWindow == null)
                return;

            if (currentWindow.CanCloseByBack == false)
                return;

            currentWindow.Close();
        }
    }
}