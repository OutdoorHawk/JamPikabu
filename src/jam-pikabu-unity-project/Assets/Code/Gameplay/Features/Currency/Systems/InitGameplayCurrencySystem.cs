using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.Result.Service;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class InitGameplayCurrencySystem : IInitializeSystem
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IResultWindowService _resultWindowService;
        private readonly IGroup<GameEntity> _storages;
        private readonly IGroup<MetaEntity> _metaGold;

        public InitGameplayCurrencySystem
        (
            GameContext context,
            MetaContext meta,
            ICurrencyFactory currencyFactory,
            
            IGameplayCurrencyService gameplayCurrencyService,
            IResultWindowService resultWindowService
        )
        {
            _currencyFactory = currencyFactory;
            _gameplayCurrencyService = gameplayCurrencyService;
            _resultWindowService = resultWindowService;

            _metaGold = meta.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Gold, 
                    MetaMatcher.Storage
                ));
            
            _storages = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.CurrencyStorage,
                    GameMatcher.CurrencyTypeId
                ));
        }

        public void Initialize()
        {
            foreach (MetaEntity gold in _metaGold)
            {
                _currencyFactory.CreateCurrencyStorages(gold.Gold);
            }
            
            foreach (var entity in _storages)
            {
                _gameplayCurrencyService.UpdateCurrencyAmount(entity.CurrencyAmount, 0, entity.CurrencyTypeId);
                _resultWindowService.InitInitialCurrency(entity.CurrencyTypeId, entity.CurrencyAmount);
            }
        }
    }
}