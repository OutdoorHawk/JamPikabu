using System.Collections.Generic;
using Code.Gameplay.Input.Service;
using Entitas;

namespace Code.Gameplay.Input.Systems
{
    
    public class RagdollInputStateReactiveSystem : ReactiveSystem<InputEntity>
    {
        private readonly IInputService _inputService;

        public RagdollInputStateReactiveSystem(Contexts contexts, IInputService inputService) : base(contexts.input)
        {
            _inputService = inputService;
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(InputMatcher.RagdollInput.AddedOrRemoved());
        }

        protected override bool Filter(InputEntity entity)
        {
            return entity.isPauseInput == false;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.isRagdollInput)
                    _inputService.RagdollStateInput();
                else
                    _inputService.EnableAllInput();
            }
        }
    }
}