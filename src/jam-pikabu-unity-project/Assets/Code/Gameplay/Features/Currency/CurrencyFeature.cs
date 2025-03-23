using Code.Gameplay.Features.Currency.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Currency
{
    public sealed class CurrencyFeature : Feature
    {
        public CurrencyFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitGameplayCurrencySystem>());
            
            Add(systems.Create<ProcessAddGoldRequestSystem>());
            Add(systems.Create<ProcessAddRatingRequestSystem>());
            
            Add(systems.Create<RefreshGoldSystem>());
            Add(systems.Create<RefreshRatingSystem>());
            
            Add(systems.Create<TransferGoldToMetaStorageOnEndDay>());
        }
    }
}