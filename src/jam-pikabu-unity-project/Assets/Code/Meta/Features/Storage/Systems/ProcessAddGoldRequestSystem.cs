using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Meta.Features.Storage.Systems
{
    public class ProcessAddGoldRequestSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _requests;
        private readonly IGroup<MetaEntity> _storages;
        private readonly ISaveLoadService _saveLoadService;

        public ProcessAddGoldRequestSystem(MetaContext context, ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
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
                
                _saveLoadService.SaveProgress();
            }
        }
    }
}