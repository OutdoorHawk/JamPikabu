using Code.Gameplay.Features.Currency.Service;
using Code.Meta.UI.Common;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class CurrencyHolder : MonoBehaviour
    {
        public PriceInfo PlayerCurrentGold;
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
            _gameplayCurrencyService.RegisterHolder(this);
        }

        private void Start()
        {
            RefreshStatic();
        }

        private void OnDestroy()
        {
            _gameplayCurrencyService.CurrencyChanged -= Refresh;
            _gameplayCurrencyService.UnregisterHolder(this);
        }

        private void RefreshStatic()
        {
            PlayerCurrentGold.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Gold), CurrencyTypeId.Gold);
            PlayerPluses.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus), CurrencyTypeId.Plus);
            PlayerMinuses.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus), CurrencyTypeId.Minus);
        }

        private void Refresh()
        {
            PlayerCurrentGold.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Gold), CurrencyTypeId.Gold, true);
            PlayerPluses.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus), CurrencyTypeId.Plus, true);
            PlayerMinuses.SetupPrice(_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus), CurrencyTypeId.Minus, true);
        }
    }
}