using Code.Meta.UI.HardCurrencyHolder.Service;
using Entitas;

namespace Code.Meta.Features.Storage.Systems
{
    public class RefreshHardSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _storages;
        private readonly IStorageUIService _storage;

        public RefreshHardSystem(MetaContext meta, IStorageUIService storage)
        {
            _storage = storage;
            _storages = meta.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Storage,
                    MetaMatcher.Hard));
        }

        public void Execute()
        {
            foreach (MetaEntity storage in _storages)
                _storage.UpdateCurrentHard(storage.Hard);
        }
    }
}