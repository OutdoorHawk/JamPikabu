using System.Collections.Generic;
using Entitas;

namespace Code.Meta.Features.Storage.Systems
{
    public class InitStorageSystem : IInitializeSystem
    {
        private readonly IGroup<MetaEntity> _storages;
        private readonly List<MetaEntity> _buffer = new(1);

        public InitStorageSystem(MetaContext context)
        {
            _storages = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Storage,
                    MetaMatcher.Gold)
                .NoneOf(MetaMatcher.Withdraw));
        }

        public void Initialize()
        {
            foreach (var storage in _storages.GetEntities(_buffer))
            {
                storage.AddWithdraw(0);
            }
        }
    }
}