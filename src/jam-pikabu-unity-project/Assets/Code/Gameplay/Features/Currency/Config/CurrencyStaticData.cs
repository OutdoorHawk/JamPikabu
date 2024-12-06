using System.Collections.Generic;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Features.Currency.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(CurrencyStaticData), fileName = nameof(CurrencyStaticData))]
    public class CurrencyStaticData : BaseStaticData
    {
        [SerializeField] private List<CurrencyConfig> _configs;
        [SerializeField] private CurrencyAnimation _currencyAnimationPrefab;

        public List<CurrencyConfig> Configs => _configs;

        public CurrencyAnimation CurrencyAnimationPrefab => _currencyAnimationPrefab;

        private readonly Dictionary<CurrencyTypeId, CurrencyConfig> _currencyConfigs = new();

        public override void OnConfigInit()
        {
            base.OnConfigInit();

            _currencyConfigs.Clear();

            foreach (var config in _configs)
                _currencyConfigs[config.CurrencyTypeId] = config;
        }

        public CurrencyConfig GetCurrencyConfig(CurrencyTypeId id)
        {
            return _currencyConfigs.GetValueOrDefault(id);
        }
    }
}