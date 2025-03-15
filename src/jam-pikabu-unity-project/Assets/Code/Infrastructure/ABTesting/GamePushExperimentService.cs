using System.Collections.Generic;
using Code.Infrastructure.Integrations;
using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;
using GamePush;

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
            return _cachedExperiments.GetValueOrDefault(tag, ExperimentValueTypeId.@default);
        }
    }
}