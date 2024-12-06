using System.Threading;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static System.Threading.CancellationTokenSource;

namespace Code.Meta.UI.Common
{
    public class PriceInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Image _currencyIcon;
        [SerializeField] private float _textAnimationDuration = 0.25f;

        private IStaticDataService _staticDataService;
        private CancellationTokenSource _animationToken = new();
        private int _currentAmount;

        public Image CurrencyIcon => _currencyIcon;

        [Inject]
        private void Construct(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public void SetupPrice(int amount, CurrencyTypeId typeId, bool withAnimation = false)
        {
            SetupPriceInternal(amount, typeId, withAnimation);
        }

        public void SetupPrice(CostSetup costSetup, bool withAnimation = false)
        {
            SetupPriceInternal(costSetup.Amount, costSetup.CurrencyType, withAnimation);
        }

        private void SetupPriceInternal(int amount, CurrencyTypeId typeId, bool withAnimation)
        {
            var staticData = _staticDataService.GetStaticData<CurrencyStaticData>();
            CurrencyConfig currency = staticData.GetCurrencyConfig(typeId);

            if (currency == null)
            {
                Debug.LogError($"Unknown currency!");
                return;
            }

            if (_currencyIcon.sprite != currency.Data.Icon)
                _currencyIcon.sprite = currency.Data.Icon;

            if (withAnimation == false)
            {
                _amountText.text = amount.ToString();
            }
            else
            {
                _animationToken?.Cancel();
                _animationToken = CreateLinkedTokenSource(destroyCancellationToken);
                
                _amountText.ToTextValue
                (
                    _currentAmount,
                    amount,
                    _textAnimationDuration,
                    _animationToken.Token,
                    format: "0"
                ).Forget();
            }

            _currentAmount = amount;
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