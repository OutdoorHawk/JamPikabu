namespace Code.Gameplay.Features.TextNotification.Service
{
    public interface INotificationTextService
    {
        void ShowNotificationText(in NotificationTextParameters parameters);
    }
}