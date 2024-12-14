using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Currency.Service;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class InitGameplayCurrency : IInitializeSystem
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly IGameplayCurrencyService _service;
        private readonly IGroup<GameEntity> _storages;
        
        private readonly IGroup<MetaEntity> _metaGold;

        public InitGameplayCurrency(ICurrencyFactory currencyFactory, MetaContext meta, GameContext gameContext, IGameplayCurrencyService service)
        {
            _currencyFactory = currencyFactory;
            _service = service;

            _storages = gameContext.GetGroup(GameMatcher.CurrencyStorage);
            _metaGold = meta.GetGroup(MetaMatcher.Gold);
        }

        public void Initialize()
        {
            foreach (MetaEntity gold in _metaGold)
            {
                _currencyFactory.CreateCurrencyStorages(gold.Gold);
            }
        }
    }
}