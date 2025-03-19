using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async UniTaskVoid LoadIntegrations()
        {
            foreach (IIntegration integration in _integrations)
            {
                await TryInitIntegration(integration);
            }
        }

        private async UniTask TryInitIntegration(IIntegration integration)
        {
            try
            {
                _loggerService.Log($"<b>[Bootstrap]</b> LoadIntegration: {integration.GetType().Name}");
                await integration.Initialize();
            }
            catch (Exception e)
            {
                _loggerService.LogError($"<b>[Bootstrap]</b> Error loading integrations: {e}");
            }
        }
    }
}