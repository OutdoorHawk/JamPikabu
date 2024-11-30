using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook.Factory
{
    public interface IGrapplingHookFactory
    {
        GameEntity CreateGrapplingHook(Transform parent);
    }
}