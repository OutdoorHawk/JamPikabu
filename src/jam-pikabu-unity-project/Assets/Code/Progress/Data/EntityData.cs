using System.Collections.Generic;
using Newtonsoft.Json;

namespace Code.Progress.Data
{
    public class EntityData
    {
        [JsonProperty("es_meta")] public List<EntitySnapshot> MetaEntitySnapshots;
    }
}