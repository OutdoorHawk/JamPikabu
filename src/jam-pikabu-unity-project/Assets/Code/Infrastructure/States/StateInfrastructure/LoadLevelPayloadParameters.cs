namespace Code.Infrastructure.States.StateInfrastructure
{
    public struct LoadLevelPayloadParameters
    {
        public string LevelName;
        public bool InstantLoad;

        public LoadLevelPayloadParameters(string levelName)
        {
            LevelName = levelName;
            InstantLoad = false;
        }
    }
}