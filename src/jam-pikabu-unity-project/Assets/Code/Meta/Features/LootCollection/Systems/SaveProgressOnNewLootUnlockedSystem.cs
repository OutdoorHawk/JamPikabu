using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class SaveProgressOnNewLootUnlockedSystem : ICleanupSystem
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGroup<MetaEntity> _unlockNewLootRequest;

        public SaveProgressOnNewLootUnlockedSystem(MetaContext context, ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _unlockNewLootRequest = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.UnlockLootRequest
                ));
        }

        public void Cleanup()
        {
            foreach (var entity in _unlockNewLootRequest)
            {
                _saveLoadService.SaveProgress();
            }
        }
    }
}