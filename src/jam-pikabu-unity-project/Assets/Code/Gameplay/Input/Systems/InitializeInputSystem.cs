using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Input.Service;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input.Systems
{
    public class InitializeInputSystem : IInitializeSystem, ITearDownSystem
    {
        private readonly IGroup<InputEntity> _entities;
        private readonly InputContext _context;
        private readonly IInputService _inputService;

        public InitializeInputSystem(InputContext context, IInputService inputService)
        {
            _context = context;
            _inputService = inputService;
        }

        public void Initialize()
        {
            InputEntity inputEntity = CreateInputEntity.Empty()
                .With(x => x.isInput = true)
                .With(x => x.isPauseInput = false)
                .AddMouseAxis(Vector2.zero)
                .AddMovementAxis(Vector2.zero);

            _inputService.CreateEcsBindings(inputEntity);
            _inputService.EnableAllInput();
        }

        public void TearDown()
        {
            _inputService.CleanupEcsBindings();
        }
    }
}