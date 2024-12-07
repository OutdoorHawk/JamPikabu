using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using Button = UnityEngine.UI.Button;

namespace Code.Gameplay.Input.Behaviours
{
    [RequireComponent(typeof(Button))]
    public class ButtonCommand : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private InputActionReference actionReference;
        [InputControl] [SerializeField] private string path;
        [SerializeField] private Key[] keys;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            foreach (var key in keys)
            {
                // Эмулируем нажатие клавиши
                InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(key));
                InputSystem.Update();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Эмулируем отпускание клавиши
            InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState());
            InputSystem.Update();
        }
    }
}