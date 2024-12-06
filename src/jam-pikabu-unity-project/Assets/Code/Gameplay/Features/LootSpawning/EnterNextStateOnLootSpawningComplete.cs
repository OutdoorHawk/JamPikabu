using System.Collections.Generic;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Entitas;

namespace Code.Gameplay.Features.LootSpawning
{
    public class EnterNextStateOnLootSpawningComplete : ReactiveSystem<GameEntity>
    {
        private readonly IGameStateMachine _gameStateMachine;

        public EnterNextStateOnLootSpawningComplete(GameContext context, IGameStateMachine gameStateMachine) : base(context)
        {
            _gameStateMachine = gameStateMachine;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.LootSpawner.Removed());
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _gameStateMachine.Enter<RoundPreparationLoopState>();
        }
    }
}