using System;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    [RequireComponent(typeof(Image))]
    public class CurrencyIconBehaviour : MonoBehaviour, IConfigsInitHandler
    {
        public CurrencyTypeId Type;
        
        private IStaticDataService _staticDataService;

        [Inject]
        private void Construct(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        private void Awake()
        {
            _staticDataService.RegisterHandler(this);
        }
        
        private void OnDestroy()
        {
            _staticDataService.UnRegisterHandler(this);
        }

        public void OnConfigsInitInitComplete()
        {
            CurrencyConfig currencyConfig = _staticDataService
                .Get<CurrencyStaticData>()
                .GetCurrencyConfig(Type);
            
            GetComponent<Image>().sprite = currencyConfig.Data.Icon;
        }
    }
}