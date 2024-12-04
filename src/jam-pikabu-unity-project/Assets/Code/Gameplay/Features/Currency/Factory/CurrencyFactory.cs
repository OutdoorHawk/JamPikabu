using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.StaticData;

namespace Code.Gameplay.Features.Currency.Factory
{
    public class CurrencyFactory : ICurrencyFactory
    {
        private readonly IStaticDataService _staticDataService;

        public CurrencyFactory(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public GameEntity CreateGoldCurrencyForCurrentLevel()
        {
            var roundStateStaticData = _staticDataService.GetStaticData<RoundStateStaticData>();
            return CreateGameEntity
                    .Empty()
                    .With(x => x.isCurrencyStorage = true)
                    .AddCurrencyTypeId(CurrencyTypeId.Gold)
                    .AddGold(roundStateStaticData.StartGoldAmount)
                    .AddWithdraw(0)
                ;
        }
    }
}