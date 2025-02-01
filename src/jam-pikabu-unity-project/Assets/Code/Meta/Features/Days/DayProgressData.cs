namespace Code.Meta.Features.Days
{
    public struct DayProgressData
    {
        public readonly int DayId;
        public readonly int StarsEarned;
        public readonly int StarsEarnedSeen;

        public DayProgressData(int dayId, int starsEarned, int starsEarnedSeen)
        {
            DayId = dayId;
            StarsEarned = starsEarned;
            StarsEarnedSeen = starsEarnedSeen;
        }
    }
}