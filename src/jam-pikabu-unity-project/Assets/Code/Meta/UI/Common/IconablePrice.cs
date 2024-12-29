using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Common
{
    public class IconablePrice : MonoBehaviour
    {
        public RectTransform IconsParent;
        public Image IconTemplate;
        public Image IconBackTemplate;

        private IStaticDataService _staticDataService;

        [Inject]
        private void Construct(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public void SetupPrice(CostSetup rating)
        {
            CurrencyConfig currencyConfig = _staticDataService.Get<CurrencyStaticData>().GetCurrencyConfig(rating.CurrencyType);
            IconTemplate.sprite = currencyConfig.Data.Icon;
            IconBackTemplate.sprite = currencyConfig.Data.Icon;

            for (int i = 0; i < rating.Amount; i++)
            {
                Instantiate(IconTemplate.gameObject, IconsParent).EnableElement();
            }
        }
    }
}