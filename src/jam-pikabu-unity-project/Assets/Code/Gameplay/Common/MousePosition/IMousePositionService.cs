using UnityEngine;

namespace Code.Gameplay.Common.MousePosition
{
    public interface IMousePositionService
    {
        void Initialize();
        Vector3 GetMouseWorldPosition();
        Camera MainCamera { get; }
    }
}