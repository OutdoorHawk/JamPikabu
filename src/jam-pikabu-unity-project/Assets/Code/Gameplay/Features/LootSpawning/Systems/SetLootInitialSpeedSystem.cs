using System.Collections.Generic;
using Code.Infrastructure.SceneContext;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class SetLootInitialSpeedSystem : ReactiveSystem<GameEntity>
    {
        private readonly ISceneContextProvider _sceneContextProvider;

        public SetLootInitialSpeedSystem(GameContext context, ISceneContextProvider sceneContextProvider) : base(context)
        {
            _sceneContextProvider = sceneContextProvider;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.Rigidbody2D
                )
                .Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isLoot && entity.hasRigidbody2D;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                Vector2 direction = entity.Rigidbody2D.transform.right;
                const float power = 1.4f;
                entity.Rigidbody2D.AddForce(direction * power, ForceMode2D.Impulse);
            }
        }
    }
}