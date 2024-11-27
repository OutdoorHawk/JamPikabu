using Code.Gameplay.Input.Service;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input.Systems
{
    public sealed class EmitInputSystem : IExecuteSystem
    {
        private readonly IInputService _inputService;
        private readonly IGroup<InputEntity> _mouseAxisEntities;
        private readonly IGroup<InputEntity> _movementAxisEntities;

        public EmitInputSystem(Contexts contexts, IInputService inputService)
        {
            _inputService = inputService;
            
            _mouseAxisEntities = contexts.input.GetGroup(InputMatcher
                .AllOf(InputMatcher.Input, InputMatcher.MouseAxis));
            
            _movementAxisEntities = contexts.input.GetGroup(InputMatcher
                .AllOf(InputMatcher.Input, InputMatcher.MovementAxis));
        }
        
        public void Execute()
        {
            foreach (var inputEntity in _mouseAxisEntities)
            {
                inputEntity.mouseAxis.Value = _inputService.MouseAxis.ReadValue<Vector2>();
            }

            foreach (var inputEntity in _movementAxisEntities)
            {
                inputEntity.movementAxis.Value = _inputService.Movement.ReadValue<Vector2>();
            }
        }
    }
}