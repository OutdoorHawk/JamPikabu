using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Integrations.Service
{
    public interface IIntegrationsService
    {
        UniTask LoadIntegrations();
    }
}