namespace Code.Infrastructure.Common.Time
{
    public interface IPauseHandler
    {
        void EnablePause();
        void DisablePause();
    }
}