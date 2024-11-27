namespace Code.Gameplay.Input
{
    public static class InputExtensions
    {
        public static int GetInputSelection(InputEntity input)
        {
            if (input.isSelection1)
                return 1;
            if (input.isSelection2)
                return 2;
            if (input.isSelection3)
                return 3;
            if (input.isSelection4)
                return 4;
            if (input.isSelection5)
                return 5;
            if (input.isSelection6)
                return 6;
            
            return -1;
        }
    }
}