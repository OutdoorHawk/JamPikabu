using Code.Gameplay.Input.Service;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using UnityEngine.InputSystem;
using Zenject;

namespace Code.Gameplay.Cheats.Service
{
    public class CheatsService : ICheatsService, IInitializable
    {
        private IWindowService _windowService;
        private IInputService _inputService;

        [Inject]
        private void Construct(IWindowService windowService, IInputService inputService)
        {
            _inputService = inputService;
            _windowService = windowService;
        }

        public void Initialize()
        {
            _inputService.PlayerInput.Cheats.ToggleCheatsWindow.Enable();
            _inputService.PlayerInput.Cheats.ToggleCheatsWindow.performed += ToggleCheats;
        }

        private void ToggleCheats(InputAction.CallbackContext obj)
        {
            if (_windowService.IsWindowOpen(WindowTypeId.Cheats))
                _windowService.Close(WindowTypeId.Cheats);
            else
                _windowService.OpenWindow(WindowTypeId.Cheats);
        }
    }
}