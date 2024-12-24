namespace Code.Infrastructure.States.GameStateHandler
{
    public enum OrderType
    {
        First = 0,
        Second = 1,
        Third = 2,
        Tutorial = 5,
        Middle = int.MaxValue / 2,
        Penultimate = int.MaxValue - 1,
        Last = int.MaxValue,
    }
}