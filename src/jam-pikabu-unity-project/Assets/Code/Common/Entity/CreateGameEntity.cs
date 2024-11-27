using Code.Infrastructure.Common.GameIdentifier;

namespace Code.Common.Entity
{
    public static class CreateGameEntity
    {
        public static GameEntity Empty()
        {
            GameEntity entity = Contexts.sharedInstance.game.CreateEntity();
            GameIdentifierService.Instance.AddGeneralId(entity);
            return entity;
        }
    }
}