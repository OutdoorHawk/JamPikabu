using Code.Gameplay.Input.Service;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Code.Gameplay.Common.MousePosition
{
    public class MousePositionService : IMousePositionService
    {
        private readonly IInputService _inputService;

        private Camera _camera;
        private Plane _virtualPlane;
        private Vector3 _mouseWorldPosition;

        private const float SPEED = 10f;

        public Camera MainCamera => _camera;

        public MousePositionService(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void Initialize()
        {
            _virtualPlane = new Plane(Vector3.up, Vector3.zero);
            _camera = Camera.main;
            //_inputService.PlayerInput.Player.Scroll.performed += UpdateZoom;
        }

        private void UpdateZoom(InputAction.CallbackContext context)
        {
            /*Vector2 scrollValue = context.ReadValue<Vector2>();
            Camera camera = _camera.CameraHandler.Camera;

            float result = Math.Clamp(camera.transform.localPosition.z + scrollValue.y * SPEED, -14, -10);
            camera.transform.DOLocalMoveZ(result, 0.25f);*/
        }

        public Vector3 GetMouseWorldPosition()
        {
            Vector2 screenPosition = GetPointerPositionIgnoringUI(); // Получаем позицию активного касания/мыши, игнорируя UI
            if (screenPosition == Vector2.zero) 
                return _mouseWorldPosition;
            
            Ray ray = MainCamera.ScreenPointToRay(screenPosition );

            if (_virtualPlane.Raycast(ray, out var distance))
            {
                Vector3 position3D = ray.GetPoint(distance);
                _mouseWorldPosition = position3D;
            }

            return _mouseWorldPosition;
        }
        
        private Vector2 GetPointerPositionIgnoringUI()
        {
            // Если сенсорный экран доступен, обрабатываем касания
            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.press.isPressed)
                    {
                        Vector2 touchPosition = touch.position.ReadValue();

                        // Проверяем, находится ли касание над UI
                        if (!IsPointerOverUI(touchPosition))
                        {
                            return touchPosition; // Возвращаем первое касание, которое не над UI
                        }
                    }
                }
            }
            // Если используется мышь
            else if (Mouse.current != null)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();

                // Проверяем, находится ли мышь над UI
                if (!IsPointerOverUI(mousePosition))
                {
                    return mousePosition;
                }
            }

            // Если ни одно касание не прошло проверку
            return Vector2.zero;
        }

        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            // Создаём PointerEventData для проверки UI
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            // Сохраняем результаты
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Если список результатов не пуст, то позиция над UI элементом
            bool isPointerOverUI = results.Count > 0;

            if (isPointerOverUI)
            {
                foreach (var result in results)
                {
                    GameObject resultGameObject = result.gameObject;
                    if (resultGameObject == null)
                        continue;
                    
                    if (resultGameObject.TryGetComponent(out VirtualJoystick joystick) == false)
                        continue;

                    if (joystick.PointerDown)
                        return true;
                }
            }

            return false;
        }
    }
}