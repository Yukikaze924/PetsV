using Newtonsoft.Json;

namespace PetsV.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Config
    {
        [JsonProperty("lang")]
        public string Lang { get; set; } = "en-us";
    }
}
