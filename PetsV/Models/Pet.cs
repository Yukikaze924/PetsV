using GTA;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static PetsV.Enums.PetEnums;

namespace PetsV.Models
{
    public class Pet
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("species")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Species Species { get; set; } = Species.Cat;

        [JsonProperty("breed")]
        public int Breed { get; set; } = 0;

        [JsonProperty("gender")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Gender Gender { get; set; } = Gender.Male;

        [JsonProperty("status")]
        public Status Status { get; set; } = (Status)0;

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonIgnore]
        public Ped Entity { get; set; }
    }
}
