using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.StaticData;

namespace Code.Gameplay.Features.Currency.Factory
{
    public class CurrencyFactory : ICurrencyFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;

        public CurrencyFactory(IStaticDataService staticDataService, IGameplayCurrencyService gameplayCurrencyService)
        {
            _staticDataService = staticDataService;
            _gameplayCurrencyService = gameplayCurrencyService;
        }

        public void CreateCurrencyStorages()
        {
            var roundStateStaticData = _staticDataService.GetStaticData<RoundStateStaticData>();
            
             CreateGameEntity
                    .Empty()
                    .With(x => x.isCurrencyStorage = true)
                    .AddCurrencyTypeId(CurrencyTypeId.Gold)
                    .AddGold(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Gold))
                    .AddWithdraw(0)
                ;
             
             CreateGameEntity
                 .Empty()
                 .With(x => x.isCurrencyStorage = true)
                 .AddCurrencyTypeId(CurrencyTypeId.Plus)
                 .AddPlus(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus))
                 .AddWithdraw(0)
                 ;
             
             CreateGameEntity
                 .Empty()
                 .With(x => x.isCurrencyStorage = true)
                 .AddCurrencyTypeId(CurrencyTypeId.Plus)
                 .AddMinus(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus))
                 .AddWithdraw(0)
                 ;
        }
    }
}