namespace Code.Infrastructure.Common.Time
{
    public interface IPauseHandler
    {
        public void EnablePause();
        public void DisablePause();
    }
}