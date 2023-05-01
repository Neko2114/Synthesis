using Newtonsoft.Json;

namespace Synthesis.Penumbra
{

    public class PenumbraMetaData
    {
        [JsonProperty("FileVersion")]
        public long FileVersion { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Author")]
        public string Author { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Version")]
        public string Version { get; set; }

        [JsonProperty("Website")]
        public string Website { get; set; }

        [JsonProperty("ModTags")]
        public object[] ModTags { get; set; }
    }
}
