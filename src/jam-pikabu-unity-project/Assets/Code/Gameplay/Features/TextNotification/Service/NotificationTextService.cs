using Code.Gameplay.Features.TextNotification.Behaviours;
using Code.Gameplay.Windows.Factory;
using Code.Infrastructure.AssetManagement.AssetProvider;
using Zenject;

namespace Code.Gameplay.Features.TextNotification.Service
{
    public class NotificationTextService : INotificationTextService
    {
        private readonly IInstantiator _instantiator;
        private readonly IAssetProvider _assetProvider;
        private readonly IUIFactory _uiFactory;

        public NotificationTextService(IInstantiator instantiator, IAssetProvider assetProvider, IUIFactory uiFactory)
        {
            _instantiator = instantiator;
            _assetProvider = assetProvider;
            _uiFactory = uiFactory;
        }

        public void ShowNotificationText(in NotificationTextParameters parameters)
        {
            NotificationText notificationText = string.IsNullOrEmpty(parameters.Prefab)
                ? _assetProvider.LoadAssetFromResources<NotificationText>("UI/Notificator/NotificationTextAnimation")
                : _assetProvider.LoadAssetFromResources<NotificationText>($"UI/Notificator/{parameters.Prefab}");

            var instance = _instantiator.InstantiatePrefabForComponent<NotificationText>(notificationText, _uiFactory.UIRoot);
            instance.Initialize(parameters);
        }
    }
}