using System;
using System.Collections.Generic;
using Code.Infrastructure.Analytics;
using Code.Infrastructure.Integrations;
using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;
using GamePush;
using UnityEngine;

namespace Code.Infrastructure.ABTesting
{
    public class GamePushExperimentService : IABTestService, IIntegration
    {
        private readonly IAnalyticsService _analyticsService;
        
        public OrderType InitOrder => OrderType.Second;

        private readonly Dictionary<ExperimentTagTypeId, ExperimentValueTypeId> _cachedExperiments = new();

        public GamePushExperimentService(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public UniTask Initialize()
        {
            for (ExperimentTagTypeId tag = 0; tag < ExperimentTagTypeId.Count; tag++)
            for (ExperimentValueTypeId value = 0; value < ExperimentValueTypeId.Count; value++)
            {
                if (GP_Experiments.Has(tag.ToString(), value.ToString()))
                    _cachedExperiments[tag] = value;
            }

            SendPlayerExperiment();
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

        private void SendPlayerExperiment()
        {
            int experimentValue = (int)GetExperimentValue(ExperimentTagTypeId.TIMER_REPLACE);
            _analyticsService.SendEvent(AnalyticsEventTypes.ExperimentTimer, experimentValue.ToString());
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