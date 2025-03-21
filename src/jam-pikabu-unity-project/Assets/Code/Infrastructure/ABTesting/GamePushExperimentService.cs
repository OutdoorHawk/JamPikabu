using System;
using System.Collections.Generic;
using Code.Infrastructure.Integrations;
using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;
using GamePush;
using UnityEngine;

namespace Code.Infrastructure.ABTesting
{
    public class GamePushExperimentService : IABTestService, IIntegration
    {
        public OrderType InitOrder => OrderType.Last;

        private readonly Dictionary<ExperimentTagTypeId, ExperimentValueTypeId> _cachedExperiments = new();

        public UniTask Initialize()
        {
            for (ExperimentTagTypeId tag = 0; tag < ExperimentTagTypeId.Count; tag++)
            for (ExperimentValueTypeId value = 0; value < ExperimentValueTypeId.Count; value++)
            {
                if (GP_Experiments.Has(tag.ToString(), value.ToString()))
                    _cachedExperiments[tag] = value;
            }

            return UniTask.CompletedTask;
        }

        public ExperimentValueTypeId GetExperimentValue(ExperimentTagTypeId tag)
        {
#if CHEAT
            string savedValue = PlayerPrefs.GetString(tag.ToString(), ExperimentValueTypeId.@default.ToString());
            return Enum.Parse<ExperimentValueTypeId>(savedValue);
#endif
            return _cachedExperiments.GetValueOrDefault(tag, ExperimentValueTypeId.@default);
        }

        /*private bool InitializeValue(ExperimentTagTypeId tag, ExperimentValueTypeId value)
        {
            string tagKey = tag.ToString();

            if (PlayerPrefs.HasKey(tagKey))
            {
                _cachedExperiments[tag] = Enum.TryParse(PlayerPrefs.GetString(tagKey), out ExperimentValueTypeId experimentValue)
                    ? experimentValue
                    : ExperimentValueTypeId.@default;

                return true;
            }

            if (GP_Experiments.Has(tag.ToString(), value.ToString()))
            {
                _cachedExperiments[tag] = value;
            }

            return false;
        }*/
    }
}