using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Service;
using Entitas;

namespace Code.Meta.Features.Storage.Systems
{
    public class RefreshGoldSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _storages;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;

        public RefreshGoldSystem(MetaContext meta, IGameplayCurrencyService gameplayCurrencyService)
        {
            _gameplayCurrencyService = gameplayCurrencyService;
            _storages = meta.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Storage,
                    MetaMatcher.Gold));
        }

        public void Execute()
        {
            foreach (MetaEntity storage in _storages)
                _gameplayCurrencyService.UpdateCurrencyAmount(storage.Gold, 0, CurrencyTypeId.Gold);
        }
    }
}