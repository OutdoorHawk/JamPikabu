using System;
using System.Collections.Generic;
using Code.Common.Extensions;
using Entitas;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Code.Gameplay.Input.Service
{
    public class InputService : IInputService
    {
        private readonly PlayerControls _playerInput;
        private readonly IInstantiator _instantiator;
        private readonly Dictionary<InputAction, int> _inputBindingsDict = new();

        private InputEntity _inputEntity;

        public InputAction Movement { get; }
        public InputAction MouseAxis { get; }

        public PlayerControls PlayerInput => _playerInput;

        public InputService(IInstantiator instantiator)
        {
            _instantiator = instantiator;
            _playerInput = new PlayerControls();
            _playerInput.Enable();
            Movement = PlayerInput.Player.MovementAxis;
            MouseAxis = PlayerInput.Player.MouseAxis;
        }

        public void CreateEcsBindings(InputEntity inputEntity)
        {
            _inputEntity = inputEntity;

            if (_inputBindingsDict.Count != 0)
                return;

            FillBindingsDictionary();
            SubscribeActionEmit();
        }

        public void CleanupEcsBindings()
        {
            _inputBindingsDict.Clear();
            UnsubscribeActionEmit();
        }

        public bool IsMobile()
        {
            return IsMobileDevice();
        }

        private void FillBindingsDictionary()
        {
            foreach (InputAction action in _playerInput.Player.Get().actions)
            {
                for (int i = 0; i < InputComponentsLookup.componentNames.Length; i++)
                {
                    string componentName = InputComponentsLookup.componentNames[i];

                    if (action.name == componentName)
                        _inputBindingsDict.Add(action, i);
                }
            }
        }

        public void EnableAllInput()
        {
            PlayerInput.Player.Enable();
            PlayerInput.UI.Enable();
        }

        public void RagdollStateInput()
        {
            PlayerInput.Player.MovementAxis.Disable();
            PlayerInput.Player.Jump.Disable();
            PlayerInput.Player.Fire.Disable();
            PlayerInput.Player.Sprint.Disable();
            PlayerInput.Player.Fire.Disable();
        }

        public void PauseStateInput()
        {
            PlayerInput.Player.MovementAxis.Disable();
            PlayerInput.Player.Jump.Disable();
            PlayerInput.Player.Fire.Disable();
            PlayerInput.Player.Sprint.Disable();
            PlayerInput.Player.Selection.Disable();
            PlayerInput.Player.Fire.Disable();
        }

        public void DisableAllInput()
        {
            PlayerInput.Player.MovementAxis.Disable();
            PlayerInput.Player.Jump.Disable();
            PlayerInput.Player.Fire.Disable();
            PlayerInput.Player.Sprint.Disable();
            PlayerInput.Player.Escape.Disable();
            PlayerInput.Player.Selection.Disable();
            PlayerInput.UI.Disable();
            PlayerInput.Player.Fire.Disable();
        }

        private void SubscribeActionEmit()
        {
            foreach (var action in PlayerInput.Player.Get())
            {
                action.started += HandleButtonPressed;
                action.canceled += HandleButtonCancelled;
            }
        }

        private void UnsubscribeActionEmit()
        {
            foreach (var action in PlayerInput.Player.Get())
            {
                action.started -= HandleButtonPressed;
                action.canceled -= HandleButtonCancelled;
            }
        }


        private void HandleButtonPressed(InputAction.CallbackContext context)
        {
            if (TryGetComponentIndexFromAction(context, out int componentIndex) == false)
                return;

            Type componentType = InputComponentsLookup.componentTypes[componentIndex];
            IComponent component = (IComponent)_instantiator.Instantiate(componentType);

            _inputEntity.With(x => x.ReplaceComponent(componentIndex, component), when: componentIndex >= 0);
        }

        private void HandleButtonCancelled(InputAction.CallbackContext context)
        {
            if (TryGetComponentIndexFromAction(context, out int componentIndex) == false)
                return;

            if (_inputEntity.HasComponent(componentIndex))
                _inputEntity.RemoveComponent(componentIndex);
        }

        private bool TryGetComponentIndexFromAction(InputAction.CallbackContext context, out int componentIndex)
        {
            if (_inputBindingsDict.TryGetValue(context.action, out componentIndex))
                return true;

            Debug.LogError($"Input component index not found for {context.action.name} action!");
            return false;
        }

        private static bool IsMobileDevice()
        {
#if UNITY_EDITOR
            return true;
            return UnityEngine.Input.touchSupported && Screen.width < 1024;
#endif
#if CRAZY_GAMES
            string deviceType = CrazyGames.CrazySDK.User.SystemInfo.device.type;
            return deviceType.Contains("mobile") || deviceType.Contains("tablet");
#endif
#if GAME_PUSH
            return GamePush.GP_Device.IsMobile();
#endif
#if UNITY_ANDROID
            return true;
#else
            return UnityEngine.Input.touchSupported && Screen.width < 1024;
#endif
        }
    }
}