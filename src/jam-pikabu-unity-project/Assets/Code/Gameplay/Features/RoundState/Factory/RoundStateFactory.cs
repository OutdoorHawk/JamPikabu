using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Configs;

namespace Code.Gameplay.Features.RoundState.Factory
{
    public class RoundStateFactory : IRoundStateFactory
    {
        private readonly IStaticDataService _staticDataService;

        public RoundStateFactory(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public GameEntity CreateRoundStateController()
        {
            var staticData = _staticDataService.Get<DaysStaticData>();

            return CreateGameEntity
                    .Empty()
                    .With(x => x.isRoundStateController = true)
                    .With(x => x.isRoundStartAvailable = true)
                    
                ;
        }
    }
}