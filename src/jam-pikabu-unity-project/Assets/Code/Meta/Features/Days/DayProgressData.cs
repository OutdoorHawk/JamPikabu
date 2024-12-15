namespace Code.Meta.Features.Days
{
    public struct DayProgressData
    {
        public int DayId;
        public int StarsEarned;

        public DayProgressData(int dayId, int starsEarned)
        {
            DayId = dayId;
            StarsEarned = starsEarned;
        }
    }
}