using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Common
{
    public class PriceInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Image _currencyIcon;

        private IStaticDataService _staticDataService;

        [Inject]
        private void Construct(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public void SetupPrice(int amount, CurrencyTypeId typeId)
        {
            SetupPriceInternal(amount, typeId);
        }

        public void SetupPrice(CostSetup costSetup)
        {
            SetupPriceInternal(costSetup.Amount, costSetup.CurrencyType);
        }

        private void SetupPriceInternal(int amount, CurrencyTypeId typeId)
        {
            var staticData = _staticDataService.GetStaticData<CurrencyStaticData>();
            
            CurrencyConfig currency = staticData.GetCurrencyConfig(typeId);

            if (currency == null)
            {
                Debug.LogError($"Unknown currency!");
                return;
            }

            _currencyIcon.sprite = currency.Data.Icon;
            _amountText.text = amount.ToString();
        }

        public void Show()
        {
            gameObject.EnableElement();
        }

        public void Hide()
        {
            gameObject.DisableElement();
        }
    }
}