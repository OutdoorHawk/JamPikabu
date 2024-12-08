using System.Collections.Generic;
using Code.Infrastructure.States.StateMachine;
using Entitas;

namespace Code.Gameplay.Features.LootSpawning.Systems
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
            return context.CreateCollector(GameMatcher
                .AllOf(
                    GameMatcher.LootSpawner,
                    GameMatcher.Complete).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
        }
    }
}