using System;
using Code.Progress.Data.Tutorial;
using Newtonsoft.Json;

namespace Code.Progress.Data
{
    [Serializable]
    public class PlayerProgress
    {
        [JsonProperty("pl")] public PlayerSettings PlayerSettings = new();
        [JsonProperty("e")] public EntityData EntityData = new();
        [JsonProperty("s")] public PlayerLevelsProgress Levels = new();
        [JsonProperty("t")] public TutorialProgress Tutorial = new();
    }
}