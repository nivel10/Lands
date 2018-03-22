namespace Lands.Models
{
    using Newtonsoft.Json;

    public class RegionalBloc
    {
        [JsonProperty(PropertyName = "acronym")]
        public string Acronym { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        //public List<object> otherAcronyms { get; set; }
        //public List<object> otherNames { get; set; }
    }
}
