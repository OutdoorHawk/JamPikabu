using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Windows.Service
{
    public interface IWindowService
    {
        IReadOnlyDictionary<WindowTypeId, BaseWindow> Windows { get; }
        UniTask<T> OpenWindow<T>(WindowTypeId type) where T : BaseWindow;
        void OpenWindow(WindowTypeId type);
        bool TryGetWindow<T>(WindowTypeId type, out T window) where T : class;
        bool TryGetWindow<T>(out T window) where T : class;
        T[] GetWindows<T>() where T : class;
        void Close(WindowTypeId type);
        bool IsWindowOpen(WindowTypeId type);
        void RemoveWindowFromHistory(BaseWindow window);
        void ClearUIRoot();
        bool AnyWindowOpen();
    }
}