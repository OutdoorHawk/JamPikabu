using Code.Gameplay.Features.Currency.Service;
using Code.Meta.UI.Common;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class CurrencyHolder : MonoBehaviour
    {
        public PriceInfo PlayerCurrentGold;
        public PriceInfo PlayerTurnCostGold;
        public PriceInfo PlayerPluses;
        public PriceInfo PlayerMinuses;

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
            PlayerCurrentGold.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Gold), CurrencyTypeId.Gold, true);
            PlayerTurnCostGold.SetupPrice(_gameplayCurrencyService.CurrentTurnCostGold, CurrencyTypeId.Gold);
            PlayerPluses.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus), CurrencyTypeId.Plus, true);
            PlayerMinuses.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus), CurrencyTypeId.Minus, true);
        }
    }
}