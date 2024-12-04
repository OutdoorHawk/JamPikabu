using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.StaticData;

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
            var staticData = _staticDataService.GetStaticData<RoundStateStaticData>();

            return CreateGameEntity
                    .Empty()
                    .With(x => x.isRoundStateController = true)
                    .With(x => x.isRoundStartAvailable = true)
                    .AddRoundDuration(staticData.RoundDuration)
                    
                ;
        }
    }
}