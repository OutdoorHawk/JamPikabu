using System;

namespace Code.Infrastructure.AssetManagement.AssetDownload
{
    public interface IAssetDownloadReporter : IProgress<float>
    {
        float Progress { get; }
        event Action ProgressUpdated;
        void Report(float value);
        void Reset();
    }
}