using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Windows.Factory
{
    public interface IUIFactory
    {
        void InitializeCamera();
        Transform UIRoot { get; }
        void CreateUiRoot();
        UniTask<T> CreateWindow<T>(WindowTypeId type) where T : BaseWindow;
    }
}