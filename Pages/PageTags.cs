using Newtonsoft.Json;

namespace Synthesis.Pages
{
    public class PageTags
    {
        [JsonProperty("head")]
        public string[] head { get; set; }
        [JsonProperty("body")]
        public string[] body { get; set; }
        [JsonProperty("legs")]
        public string[] legs { get; set; }
        [JsonProperty("shoes")]
        public string[] shoes { get; set; }
        [JsonProperty("accessories")]
        public string[] accessories { get; set; }
        [JsonProperty("animation")]
        public string[] animation { get; set; }
        [JsonProperty("bdsm")]
        public string[] bdsm { get; set; }
        [JsonProperty("hair")]
        public string[] hair { get; set; }
        [JsonProperty("piercing")]
        public string[] piercing { get; set; }
        [JsonProperty("hands")]
        public string[] hands { get; set; }
        [JsonProperty("swimsuit")]
        public string[] swimsuit { get; set; }
        [JsonProperty("extras")]
        public string[] extras { get; set; }
        [JsonProperty("options")]
        public string[] options { get; set; }
        
        [JsonProperty("presets")]
        public string[][] presets { get; set; }
    }
}
