namespace Code.Infrastructure.Common.GameIdentifier
{
    public interface IIdentifierService
    {
        int Next(Identity identity);
        void AddGeneralId(GameEntity entity);
        void AddMetaId(MetaEntity entity);
    }
}