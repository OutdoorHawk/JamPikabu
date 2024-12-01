using System.Collections.Generic;
using Code.Infrastructure.SceneContext;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class InitLootSpeedSystem : ReactiveSystem<GameEntity>
    {
        private readonly ISceneContextProvider _sceneContextProvider;

        public InitLootSpeedSystem(GameContext context, ISceneContextProvider sceneContextProvider) : base(context)
        {
            _sceneContextProvider = sceneContextProvider;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.Rigidbody2D,
                    GameMatcher.TargetParent
                )
                .Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isLoot && entity.hasRigidbody2D && entity.hasTargetParent;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                Vector2 direction = entity.Rigidbody2D.transform.right;
                const float power = 0.9f;
                entity.Rigidbody2D.AddForce(direction * power, ForceMode2D.Impulse);
            }
        }
    }
}