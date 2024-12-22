namespace Code.Infrastructure.Analytics
{
    public interface IAnalyticsService
    {
        void SendEvent(string eventName, string value);
    }
}