using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Factory;
using Zenject;

namespace Code.Gameplay.Features.Currency.Factory
{
    public class CurrencyFactory : ICurrencyFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IUIFactory _uiFactory;
        private readonly IInstantiator _instantiator;

        public CurrencyFactory(IStaticDataService staticDataService, IGameplayCurrencyService gameplayCurrencyService,
            IUIFactory uiFactory, IInstantiator instantiator)
        {
            _staticDataService = staticDataService;
            _gameplayCurrencyService = gameplayCurrencyService;
            _uiFactory = uiFactory;
            _instantiator = instantiator;
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

        public void PlayCurrencyAnimation(in CurrencyAnimationParameters parameters)
        {
            var currencyConfig = _staticDataService.GetStaticData<CurrencyStaticData>();
            CurrencyAnimation instance = _instantiator
                .InstantiatePrefabForComponent<CurrencyAnimation>(currencyConfig.CurrencyAnimationPrefab, _uiFactory.UIRoot);
            
            instance.Initialize(parameters);
        }
    }
}