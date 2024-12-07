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

        public InitGameplayCurrency(ICurrencyFactory currencyFactory, GameContext gameContext, IGameplayCurrencyService service)
        {
            _currencyFactory = currencyFactory;
            _service = service;

            _storages = gameContext.GetGroup(GameMatcher.CurrencyStorage);
        }

        public void Initialize()
        {
            foreach (var storage in _storages)
            {
                storage.isDestructed = true;
            }
            
            _currencyFactory.CreateCurrencyStorages();
        }
    }
}