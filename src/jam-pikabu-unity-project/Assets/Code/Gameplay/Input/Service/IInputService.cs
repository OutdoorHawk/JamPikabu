using UnityEngine.InputSystem;

namespace Code.Gameplay.Input.Service
{
    public interface IInputService
    {
        InputAction Movement { get; }
        InputAction MouseAxis { get; }
        PlayerControls PlayerInput { get; }
        void CreateEcsBindings(InputEntity inputEntity);
        void EnableAllInput();
        void DisableAllInput();
        void RagdollStateInput();
        void PauseStateInput();
        void CleanupEcsBindings();
        bool IsMobile();
    }
}