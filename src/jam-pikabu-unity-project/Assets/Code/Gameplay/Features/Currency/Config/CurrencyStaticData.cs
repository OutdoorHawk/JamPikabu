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
        [SerializeField] private CurrencyAnimationData[] _currencyAnimationPrefab;

        public List<CurrencyConfig> Configs => _configs;

        private readonly Dictionary<CurrencyTypeId, CurrencyConfig> _currencyConfigs = new();
        private readonly Dictionary<string, CurrencyAnimation> _currencyAnimations = new();

        public override void OnConfigInit()
        {
            base.OnConfigInit();

            _currencyConfigs.Clear();

            foreach (var config in _configs)
                _currencyConfigs[config.CurrencyTypeId] = config;
            foreach (var config in _currencyAnimationPrefab)
                _currencyAnimations[config.Name] = config.Prefab;
        }

        public CurrencyConfig GetCurrencyConfig(CurrencyTypeId id)
        {
            return _currencyConfigs.GetValueOrDefault(id);
        }
        
        public CurrencyAnimation GetCurrencyAnimation(string name)
        {
            return _currencyAnimations.GetValueOrDefault(name);
        }
    }
}