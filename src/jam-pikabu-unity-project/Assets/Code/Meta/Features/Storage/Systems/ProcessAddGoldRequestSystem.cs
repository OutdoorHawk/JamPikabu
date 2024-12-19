using Entitas;

namespace Code.Meta.Features.Storage.Systems
{
    public class ProcessAddGoldRequestSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _requests;
        private readonly IGroup<MetaEntity> _storages;

        public ProcessAddGoldRequestSystem(MetaContext context)
        {
            _requests = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Gold,
                    MetaMatcher.AddCurrencyToStorageRequest
                ));

            _storages = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Storage,
                    MetaMatcher.Gold));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            foreach (var storage in _storages)
            {
                request.isDestructed = true;
                storage.ReplaceGold(storage.Gold + request.Gold);
                
                if (request.hasWithdraw)
                    storage.ReplaceWithdraw(storage.Withdraw + request.Withdraw);
            }
        }
    }
}