using Code.Infrastructure.Common.GameIdentifier;

namespace Code.Common.Entity
{
    public static class CreateMetaEntity
    {
        public static MetaEntity Empty()
        {
            MetaEntity entity = Contexts.sharedInstance.meta.CreateEntity();
            GameIdentifierService.Instance.AddMetaId(entity);
            return entity;
        }
    }
}