using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Infrastructure.View.Systems
{
    public class SetupParentForViewSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(32);

        public SetupParentForViewSystem(GameContext game)
        {
            _entities = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.View, 
                    GameMatcher.Transform,
                    GameMatcher.TargetParent));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                entity.Transform.SetParent(entity.TargetParent);
                entity.Transform.localPosition = Vector3.zero;
                entity.Transform.rotation = Quaternion.identity;

                entity.RemoveTargetParent();
            }
        }
    }
}