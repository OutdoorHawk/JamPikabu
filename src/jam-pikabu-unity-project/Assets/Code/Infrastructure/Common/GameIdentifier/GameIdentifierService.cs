using System.Collections.Generic;

namespace Code.Infrastructure.Common.GameIdentifier
{
    public class GameIdentifierService : IIdentifierService
    {
        private readonly Dictionary<Identity, int> _identifiers = new();

        private static GameIdentifierService _instance;
        
        public static GameIdentifierService Instance => GetOrInitNew();

        private static GameIdentifierService GetOrInitNew()
        {
            _instance ??= new GameIdentifierService();
            return _instance;
        }

        public int Next(Identity identity)
        {
            int last = _identifiers.GetValueOrDefault(identity, 0);
            int next = ++last;

            _identifiers[identity] = next;

            return next;
        }

        public void AddGeneralId(GameEntity entity)
        {
            if (entity.hasId)
                return;

            entity.AddId(Next(Identity.General));
        }
        
        public void AddMetaId(MetaEntity entity)
        {
            if (entity.hasId)
                return;

            entity.AddId(Next(Identity.Meta));
        }
    }
}