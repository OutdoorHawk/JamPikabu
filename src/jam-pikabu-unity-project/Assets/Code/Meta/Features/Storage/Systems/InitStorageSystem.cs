using Entitas;

namespace Code.Meta.Features.Storage.Systems
{
    public class InitStorageSystem : IInitializeSystem
    {
        private readonly IGroup<MetaEntity> _storages;

        public InitStorageSystem(MetaContext context)
        {
            _storages = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Storage,
                    MetaMatcher.Gold));
        }

        public void Initialize()
        {
            foreach (var storage in _storages)
            {
                storage.AddWithdraw(0);
            }
        }
    }
}