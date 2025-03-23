using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Loot;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Features.Result.Service
{
    public interface IResultWindowService
    {
        int CurrentDay { get; }
        void InitInitialCurrency(CurrencyTypeId type, int amount);
        UniTask TryShowProfitWindow();
        void AddCollectedLoot(LootTypeId lootTypeId);
        IReadOnlyDictionary<LootTypeId, int> GetCollectedLoot();
        int GetCollectedCurrency(CurrencyTypeId currencyType);
        int GetTotalRating();
        bool CheckGameWin();
    }
}