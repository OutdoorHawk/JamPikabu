using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;

namespace Code.Meta.UI.PreviewItem.Service
{
    public class PreviewWindowService : IPreviewWindowService
    {
        private readonly IWindowService _windowService;

        public PreviewWindowService(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public void ShowWindow(in PreviewItemWindowParameters parameters)
        {
            OpenPreviewWindowAsync(parameters).Forget();
        }

        private async UniTaskVoid OpenPreviewWindowAsync(PreviewItemWindowParameters parameters)
        {
            var window = await _windowService.OpenWindow<PreviewItemWindow>(WindowTypeId.PreviewItem);
            window.Init(parameters);
        }
    }
}