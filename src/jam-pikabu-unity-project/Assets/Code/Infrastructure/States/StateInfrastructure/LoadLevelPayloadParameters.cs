using System;

namespace Code.Infrastructure.States.StateInfrastructure
{
    public struct LoadLevelPayloadParameters
    {
        public string LevelName;
        public bool InstantLoad;
        public Action LoadCallback;

        public LoadLevelPayloadParameters(string levelName, Action loadCallback = null)
        {
            LevelName = levelName;
            LoadCallback = loadCallback;
            InstantLoad = false;
        }
    }
}