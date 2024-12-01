namespace Code.Common
{
    public static class CommonExtensions
    {
        public static bool IsNullOrDestructed(this GameEntity entity)
        {
            if (entity == null)
                return true;
            
            if (entity.isDestructed)
                return true;

            return false;
        }
    }
}