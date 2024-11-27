using System.Collections.Generic;
using Code.Gameplay.Input.Service;
using Entitas;

namespace Code.Gameplay.Input.Systems
{
    
    public class PauseInputStateReactiveSystem : ReactiveSystem<InputEntity>
    {
        private readonly IInputService _inputService;

        public PauseInputStateReactiveSystem(Contexts contexts, IInputService inputService) : base(contexts.input)
        {
            _inputService = inputService;
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(InputMatcher.PauseInput.AddedOrRemoved());
        }

        protected override bool Filter(InputEntity entity)
        {
            return true;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.isPauseInput)
                    _inputService.PauseStateInput();
                else
                    _inputService.EnableAllInput();
            }
        }
    }
}