using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Meta.Features.Days.Systems
{
    public class SyncDayStarsSeenSystem : IExecuteSystem
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGroup<MetaEntity> _days;
        private readonly IGroup<MetaEntity> _requests;

        public SyncDayStarsSeenSystem(MetaContext context, ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _requests = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.SyncSeenStarsRequest));

            _days = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Day));
        }
        
        public void Execute()
        {
            foreach (MetaEntity request in _requests)
            {
                request.isDestructed = true;
                bool applied = false;

                foreach (MetaEntity day in _days)
                {
                    if (day.StarsAmount == day.StarsAmountSeen)
                        continue;

                    applied = true;
                    day.ReplaceStarsAmountSeen(day.StarsAmount);
                }

                if (applied)
                    _saveLoadService.SaveProgress();
            }
        }
    }
}