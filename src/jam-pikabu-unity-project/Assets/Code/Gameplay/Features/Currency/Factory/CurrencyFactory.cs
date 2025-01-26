using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Factory;
using UnityEngine;
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

        public void CreateCurrencyStorages(int goldGold)
        {
            CreateGameEntity
                .Empty()
                .With(x => x.isCurrencyStorage = true)
                .AddCurrencyTypeId(CurrencyTypeId.Gold)
                .AddGold(goldGold)
                .AddEarnedInDay(0)
                .AddWithdraw(0)
                ;

            CreateGameEntity
                .Empty()
                .With(x => x.isCurrencyStorage = true)
                .AddCurrencyTypeId(CurrencyTypeId.Plus)
                .AddPlus(0)
                .AddWithdraw(0)
                ;

            CreateGameEntity
                .Empty()
                .With(x => x.isCurrencyStorage = true)
                .AddCurrencyTypeId(CurrencyTypeId.Minus)
                .AddMinus(0)
                .AddWithdraw(0)
                ;
        }

        public void PlayCurrencyAnimation(in CurrencyAnimationParameters parameters)
        {
            var currencyConfig = _staticDataService.Get<CurrencyStaticData>();

            CurrencyAnimation currencyAnimation = string.IsNullOrEmpty(parameters.AnimationName)
                ? currencyConfig.GetCurrencyAnimation("Default")
                : currencyConfig.GetCurrencyAnimation(parameters.AnimationName);

            var instance = _instantiator.InstantiatePrefabForComponent<CurrencyAnimation>(currencyAnimation, _uiFactory.UIRoot);
            instance.Initialize(parameters);
        }

        public void CreateAddCurrencyRequest(CurrencyTypeId type, int amount, int withdraw = 0)
        {
            GameEntity request = CreateGameEntity.Empty()
                    .With(x => x.isAddCurrencyRequest = true)
                    .With(x => x.AddWithdraw(withdraw), when: withdraw != 0)
                ;

            switch (type)
            {
                case CurrencyTypeId.Unknown:
                    break;
                case CurrencyTypeId.Gold:
                    request.With(x => x.AddGold(amount));
                    break;
                case CurrencyTypeId.Plus:
                    request.With(x => x.AddPlus(amount));
                    break;
                case CurrencyTypeId.Minus:
                    request.With(x => x.AddMinus(amount));
                    break;
            }
        }
        
    }
}