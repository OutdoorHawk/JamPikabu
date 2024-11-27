using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Meta.Features.Storage.Systems
{
    public class ProcessAddHardRequestSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _requests;
        private readonly IGroup<MetaEntity> _storages;
        private readonly ISaveLoadService _saveLoadService;

        public ProcessAddHardRequestSystem(MetaContext context, ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _requests = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Hard,
                    MetaMatcher.AddCurrencyToStorageRequest
                ));

            _storages = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Storage,
                    MetaMatcher.Hard));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            foreach (var storage in _storages)
            {
                request.isDestructed = true;
                storage.ReplaceHard(storage.Hard + request.Hard);
                _saveLoadService.SaveProgress();
            }
        }
    }
}