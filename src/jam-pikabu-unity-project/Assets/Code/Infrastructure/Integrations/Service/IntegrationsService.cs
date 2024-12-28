using System;
using System.Collections.Generic;
using Code.Common.Logger.Service;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Integrations.Service
{
    public class IntegrationsService : IIntegrationsService
    {
        private readonly ILoggerService _loggerService;
        private readonly List<IIntegration> _integrations;

        public IntegrationsService
        (
            ILoggerService loggerService,
            List<IIntegration> integrations
        )
        {
            _loggerService = loggerService;
            _integrations = integrations;
        }

        public async UniTask LoadIntegrations()
        {
            try
            {
                foreach (IIntegration integration in _integrations)
                {
                    _loggerService.Log($"<b>[Bootstrap]</b> LoadIntegration: {integration.GetType().Name}");
                    await integration.Initialize();
                }
            }
            catch (Exception e)
            {
                _loggerService.LogError($"<b>[Bootstrap]</b> Error loading integrations: {e}");
            }
        }
    }
}