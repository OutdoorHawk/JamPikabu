using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Windows.Factory
{
    public interface IUIFactory
    {
        Transform UIRoot { get; }
        Canvas Canvas { get; }
        void InitializeCamera();
        void CreateUiRoot();
        void SetRaycastAvailable(bool available);
        UniTask<T> CreateWindow<T>(WindowTypeId type) where T : BaseWindow;
        Vector3 GetWorldPositionForUI(Vector3 worldPos);
        Vector3 GetWorldPositionFromScreenPosition(Vector3 screenPos);
    }
}