using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Integrations.Service
{
    public interface IIntegrationsService
    {
        UniTaskVoid LoadIntegrations();
    }
}