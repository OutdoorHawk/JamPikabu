using System;
using Code.Common.Entity;

namespace Code.Infrastructure.View
{
    public class SelfInitializedView : EntityDependant
    {
        protected override void Awake()
        {
            base.Awake();
            RegisterEntity();
        }

        private void RegisterEntity()
        {
            GameEntity entity = CreateGameEntity.Empty();
            EntityView.SetEntity(entity);
        }
    }
}