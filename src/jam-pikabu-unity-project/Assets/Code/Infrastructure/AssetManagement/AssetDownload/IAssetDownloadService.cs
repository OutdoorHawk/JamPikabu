using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.AssetManagement.AssetDownload
{
    public interface IAssetDownloadService
    {
        UniTask InitializeDownloadDataAsync();
        float GetDownloadSizeMb();
        UniTask UpdateContentAsync();
    }
}