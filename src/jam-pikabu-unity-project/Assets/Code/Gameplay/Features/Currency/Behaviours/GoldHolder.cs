using Code.Gameplay.Features.Currency.Service;
using Code.Meta.UI.Common;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class GoldHolder : MonoBehaviour
    {
        public PriceInfo PlayerCurrentGold;
        public PriceInfo PlayerTurnCostGold;

        private IGameplayCurrencyService _gameplayCurrencyService;

        [Inject]
        private void Construct(IGameplayCurrencyService gameplayCurrencyService)
        {
            _gameplayCurrencyService = gameplayCurrencyService;
        }

        private void Awake()
        {
            _gameplayCurrencyService.CurrencyChanged += Refresh;
        }

        private void Start()
        {
            Refresh();
        }

        private void OnDestroy()
        {
            _gameplayCurrencyService.CurrencyChanged -= Refresh;
        }

        private void Refresh()
        {
            PlayerCurrentGold.SetupPrice(_gameplayCurrencyService.CurrentGoldCurrency, _gameplayCurrencyService.GoldCurrencyType);
            PlayerTurnCostGold.SetupPrice(_gameplayCurrencyService.CurrentTurnCostGold, _gameplayCurrencyService.GoldCurrencyType);
        }
    }
}